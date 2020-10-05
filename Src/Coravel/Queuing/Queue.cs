using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;
using Coravel.Invocable;
using Coravel.Queuing.Broadcast;
using Coravel.Queuing.Interfaces;
using Coravel.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Coravel.Queuing
{
    public class Queue : IQueue, IQueueConfiguration
    {
        private ConcurrentQueue<ActionOrAsyncFunc> _tasks = new ConcurrentQueue<ActionOrAsyncFunc>();
        private ConcurrentDictionary<Guid, CancellationTokenSource> _tokens = new ConcurrentDictionary<Guid, CancellationTokenSource>();
        private Action<Exception> _errorHandler;

        private ILogger<IQueue> _logger;
        private IServiceScopeFactory _scopeFactory;
        private IDispatcher _dispatcher;
        private int _queueIsConsumming = 0;
        private int _tasksRunningCount = 0;

        public Queue(IServiceScopeFactory scopeFactory, IDispatcher dispatcher)
        {
            this._scopeFactory = scopeFactory;
            this._dispatcher = dispatcher;
        }

        public Guid QueueTask(Action task)
        {
            var job = new ActionOrAsyncFunc(task);
            this._tasks.Enqueue(job);
            return job.Guid;
        }

        public Guid QueueInvocable<T>() where T : IInvocable
        {
            var job = EnqueueInvocable<T>();
            return job.Guid;
        }

        public Guid QueueInvocableWithPayload<T, TParams>(TParams payload) where T : IInvocable, IInvocableWithPayload<TParams>
        {
            var job = this.EnqueueInvocable<T>(invocable => {
                IInvocableWithPayload<TParams> invocableWithParams = (IInvocableWithPayload<TParams>) invocable;
                invocableWithParams.Payload = payload;
            });
            return job.Guid;
        }

        public (Guid, CancellationTokenSource) QueueCancellableInvocable<T>() where T : IInvocable, ICancellableTask
        {
            var tokenSource = new CancellationTokenSource();
            var func = this.EnqueueInvocable<T>((invocable) => {
                (invocable as ICancellableTask).Token = tokenSource.Token;
            });
            this._tokens.TryAdd(func.Guid, tokenSource);
            return (func.Guid, tokenSource);
        }

        public Guid QueueAsyncTask(Func<Task> asyncTask)
        {
            var job = new ActionOrAsyncFunc(asyncTask);
            this._tasks.Enqueue(job);
            return job.Guid;
        }

        public void QueueBroadcast<TEvent>(TEvent toBroadcast) where TEvent : IEvent
        {
            this.QueueAsyncTask(async () => await this._dispatcher.Broadcast(toBroadcast));
        }

        public IQueueConfiguration OnError(Action<Exception> errorHandler)
        {
            this._errorHandler = errorHandler;
            return this;
        }

        public IQueueConfiguration LogQueuedTaskProgress(ILogger<IQueue> logger)
        {
            this._logger = logger;
            return this;
        }

        public async Task ConsumeQueueAsync()
        {
            try
            {
                Interlocked.Increment(ref this._queueIsConsumming);

                await this.TryDispatchEvent(new QueueConsumationStarted());

                var dequeuedTasks = this.DequeueAllTasks();
                var dequeuedGuids = dequeuedTasks.Select(t => t.Guid);

                await Task.WhenAll(
                    dequeuedTasks
                        .Select(InvokeTask)
                        .ToArray()
                );

                this.CleanTokens(dequeuedGuids);

                await this.TryDispatchEvent(new QueueConsumationEnded());
            }
            finally
            {
                Interlocked.Decrement(ref this._queueIsConsumming);
            }
        }

        public async Task ConsumeQueueOnShutdown() 
        {
            this.CancelAllTokens();
            await this.ConsumeQueueAsync();
        }
        public bool IsRunning => this._queueIsConsumming > 0;

        public QueueMetrics GetMetrics()
        {
            int runningCount = this._tasks.Count();
            return new QueueMetrics(this._tasksRunningCount, runningCount);
        }

        private void CancelAllTokens()
        {
            foreach(var kv in this._tokens.AsEnumerable())
            {
                if(!kv.Value.IsCancellationRequested)
                {
                    kv.Value.Cancel();
                }
            }
        }

        private ActionOrAsyncFunc EnqueueInvocable<T>(Action<IInvocable> beforeInvoked = null) where T : IInvocable
        {
            var func = new ActionOrAsyncFunc(async () =>
                {
                    Type invocableType = typeof(T);
                    // This allows us to scope the scheduled IInvocable object
                    /// and allow DI to inject it's dependencies.
                    using (var scope = this._scopeFactory.CreateScope())
                    {
                        if (scope.ServiceProvider.GetService(invocableType) is IInvocable invocable)
                        {                            
                            if(beforeInvoked != null)
                            {                            
                                beforeInvoked(invocable);
                            }

                            await invocable.Invoke();
                        }
                        else
                        {
                            this._logger?.LogError($"Queued invocable {invocableType} is not a registered service.");
                            throw new Exception($"Queued invocable {invocableType} is not a registered service.");
                        }
                    }
                });
            this._tasks.Enqueue(func);
            return func;
        }

        private void CleanTokens(IEnumerable<Guid> guidsForTokensToClean)
        {
            foreach(var guid in guidsForTokensToClean)
            {
                if(this._tokens.TryRemove(guid, out var token))
                {
                    token.Dispose();
                }
            }                   
        }

        private List<ActionOrAsyncFunc> DequeueAllTasks()
        {
            List<ActionOrAsyncFunc> dequeuedTasks = new List<ActionOrAsyncFunc>(this._tasks.Count());
            while (this._tasks.TryPeek(out var dummy))
            {
                this._tasks.TryDequeue(out var dequeuedTask);
                dequeuedTasks.Add(dequeuedTask);
            }
            return dequeuedTasks;
        }

        private async Task TryDispatchEvent(IEvent toBroadcast)
        {
            if (this._dispatcher != null)
            {
                await this._dispatcher.Broadcast(toBroadcast);
            }
        }

        private async Task InvokeTask(ActionOrAsyncFunc task)
        {
            try
            {
                Interlocked.Increment(ref this._tasksRunningCount);
                this._logger?.LogInformation("Queued task started...");
                await this.TryDispatchEvent(new QueueTaskStarted(task.Guid));

                await task.Invoke();

                this._logger?.LogInformation("Queued task finished...");
                await this.TryDispatchEvent(new QueueTaskCompleted(task.Guid));
            }
            catch (Exception e)
            {
                await this.TryDispatchEvent(new DequeuedTaskFailed(task));

                _errorHandler?.Invoke(e);
            }
            finally
            {
                Interlocked.Decrement(ref this._tasksRunningCount);
            }
        }
    }
}