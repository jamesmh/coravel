using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using Coravel.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Coravel.Queuing
{
    public class Queue : IQueue, IQueueConfiguration
    {
        private ConcurrentQueue<ActionOrAsyncFunc> _tasks = new ConcurrentQueue<ActionOrAsyncFunc>();
        private Action<Exception> _errorHandler;
        private ILogger<IQueue> _logger;
        private IServiceScopeFactory _scopeFactory;

        public Queue(IServiceScopeFactory scopeFactory)
        {
            this._scopeFactory = scopeFactory;
        }

        public void QueueTask(Action task)
        {
            this._tasks.Enqueue(new ActionOrAsyncFunc(task));
        }

        public void QueueInvocable<T>() where T : IInvocable
        {
            this._tasks.Enqueue(
                new ActionOrAsyncFunc(async () =>
                {
                    // This allows us to scope the scheduled IInvocable object
                    /// and allow DI to inject it's dependencies.
                    using (var scope = this._scopeFactory.CreateScope())
                    {
                        if (scope.ServiceProvider.GetRequiredService(typeof(T)) is IInvocable invocable)
                        {
                            await invocable.Invoke();
                        }
                    }
                })
            );
        }

        public void QueueAsyncTask(Func<Task> asyncTask)
        {
            this._tasks.Enqueue(new ActionOrAsyncFunc(asyncTask));
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
            IEnumerable<ActionOrAsyncFunc> queuedTasks = this.DequeueAllTasks();

            foreach (ActionOrAsyncFunc task in queuedTasks)
            {
                try
                {
                    this._logger?.LogInformation("Queued task started...");
                    await task.Invoke();
                    this._logger?.LogInformation("Queued task finished...");
                }
                catch (Exception e)
                {
                    if (this._errorHandler != null)
                    {
                        this._errorHandler(e);
                    }
                }
            }
        }

        private IEnumerable<ActionOrAsyncFunc> DequeueAllTasks()
        {
            while (this._tasks.TryPeek(out var dummy))
            {
                this._tasks.TryDequeue(out var queuedTask);
                yield return queuedTask;
            }
        }
    }
}