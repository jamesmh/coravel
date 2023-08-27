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

namespace Coravel.Queuing;

public sealed class Queue : IQueue, IQueueConfiguration
{
    private readonly ConcurrentQueue<ActionOrAsyncFunc> _tasks = new();
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _tokens = new();
    private Action<Exception>? _errorHandler;

    private ILogger<IQueue>? _logger;
    private readonly IServiceScopeFactory? _scopeFactory;
    private readonly IDispatcher _dispatcher;
    private int _queueIsConsuming = 0;
    private int _tasksRunningCount = 0;

    public Queue(IServiceScopeFactory? scopeFactory, IDispatcher dispatcher)
    {
        _scopeFactory = scopeFactory;
        _dispatcher = dispatcher;
    }

    public Guid QueueTask(Action task)
    {
        var job = new ActionOrAsyncFunc(task);
        _tasks.Enqueue(job);
        return job.Guid;
    }

    public Guid QueueInvocable<T>() where T : IInvocable
    {
        var job = EnqueueInvocable<T>();
        return job.Guid;
    }

    public Guid QueueInvocableWithPayload<T, TParams>(TParams payload) where T : IInvocable, IInvocableWithPayload<TParams>
    {
        var job = EnqueueInvocable<T>(invocable =>
        {
            var invocableWithParams = (IInvocableWithPayload<TParams>)invocable;
            invocableWithParams.Payload = payload;
        });
        return job.Guid;
    }

    public (Guid, CancellationTokenSource) QueueCancellableInvocable<T>() where T : IInvocable, ICancellableTask
    {
        var tokenSource = new CancellationTokenSource();
        var func = EnqueueInvocable<T>((invocable) =>
        {
            (invocable as ICancellableTask ?? throw new ArgumentNullException(nameof(invocable))).Token = tokenSource.Token;
        });
        _tokens.TryAdd(func.Guid, tokenSource);
        return (func.Guid, tokenSource);
    }

    public Guid QueueAsyncTask(Func<Task> asyncTask)
    {
        var job = new ActionOrAsyncFunc(asyncTask);
        _tasks.Enqueue(job);
        return job.Guid;
    }

    public void QueueBroadcast<TEvent>(TEvent toBroadcast) where TEvent : IEvent
    {
        QueueAsyncTask(async () => await _dispatcher.Broadcast(toBroadcast));
    }

    public IQueueConfiguration OnError(Action<Exception> errorHandler)
    {
        _errorHandler = errorHandler;
        return this;
    }

    public IQueueConfiguration LogQueuedTaskProgress(ILogger<IQueue> logger)
    {
        _logger = logger;
        return this;
    }

    public async Task ConsumeQueueAsync()
    {
        try
        {
            Interlocked.Increment(ref _queueIsConsuming);

            await TryDispatchEvent(new QueueConsumationStarted());

            var dequeuedTasks = DequeueAllTasks();
            var dequeuedGuids = dequeuedTasks.Select(t => t.Guid);

            await Task.WhenAll(
                dequeuedTasks
                    .Select(InvokeTask)
                    .ToArray()
            );

            CleanTokens(dequeuedGuids);

            await TryDispatchEvent(new QueueConsumationEnded());
        }
        finally
        {
            Interlocked.Decrement(ref _queueIsConsuming);
        }
    }

    public async Task ConsumeQueueOnShutdown()
    {
        CancelAllTokens();
        await ConsumeQueueAsync();
    }
    public bool IsRunning => _queueIsConsuming > 0;

    public QueueMetrics GetMetrics()
    {
        // See https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentqueue-1.count?view=net-5.0#remarks
        var waitingCount = _tasks.IsEmpty
            ? 0
            : _tasks.Count;

        return new QueueMetrics(_tasksRunningCount, waitingCount);
    }

    private void CancelAllTokens()
    {
        foreach (var kv in _tokens.Where(kv => !kv.Value.IsCancellationRequested))
        {
            kv.Value.Cancel();
        }
    }

    private ActionOrAsyncFunc EnqueueInvocable<T>(Action<IInvocable>? beforeInvoked = null) where T : IInvocable
    {
        var func = new ActionOrAsyncFunc(async () =>
            {
                Type invocableType = typeof(T);
                // This allows us to scope the scheduled IInvocable object
                // and allow DI to inject it's dependencies.
                await using var scope = _scopeFactory?.CreateAsyncScope();

                if (scope?.ServiceProvider.GetService(invocableType) is IInvocable invocable)
                {
                    beforeInvoked?.Invoke(invocable);

                    await invocable.Invoke();
                }
                else
                {
                    if (_logger != null && _logger.IsEnabled(LogLevel.Error))
                    {
                        _logger.LogError("Queued invocable {InvocableType} is not a registered service.", invocableType);
                    }

                    throw new QueueEnqueueInvocableException($"Queued invocable {invocableType} is not a registered service.");
                }
            });

        _tasks.Enqueue(func);
        return func;
    }

    private void CleanTokens(IEnumerable<Guid> guidsForTokensToClean)
    {
        foreach (var guid in guidsForTokensToClean)
        {
            if (_tokens.TryRemove(guid, out var token))
            {
                token.Dispose();
            }
        }
    }

    private List<ActionOrAsyncFunc> DequeueAllTasks()
    {
        List<ActionOrAsyncFunc> dequeuedTasks = new(_tasks.Count);

        while (_tasks.TryPeek(out _))
        {
            _tasks.TryDequeue(out var dequeuedTask);

            if (dequeuedTask != null)
            {
                dequeuedTasks.Add(dequeuedTask);
            }
        }

        return dequeuedTasks;
    }

    private async Task TryDispatchEvent(IEvent toBroadcast)
    {
        if (_dispatcher != null)
        {
            await _dispatcher.Broadcast(toBroadcast);
        }
    }

    private async Task InvokeTask(ActionOrAsyncFunc task)
    {
        try
        {
            Interlocked.Increment(ref _tasksRunningCount);

            if (_logger != null && _logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Queued task started...");
            }

            await TryDispatchEvent(new QueueTaskStarted(task.Guid));

            await task.Invoke();

            if (_logger != null && _logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Queued task finished...");
            }

            await TryDispatchEvent(new QueueTaskCompleted(task.Guid));
        }
        catch (Exception e)
        {
            await TryDispatchEvent(new DequeuedTaskFailed(task));

            _errorHandler?.Invoke(e);
        }
        finally
        {
            Interlocked.Decrement(ref _tasksRunningCount);
        }
    }
}
