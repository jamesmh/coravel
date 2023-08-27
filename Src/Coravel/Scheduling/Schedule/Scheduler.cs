using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Coravel.Scheduling.Schedule.Event;
using Microsoft.Extensions.DependencyInjection;
using Coravel.Invocable;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Broadcast;
using System.Threading;

namespace Coravel.Scheduling.Schedule;

public sealed class Scheduler : IScheduler, ISchedulerConfiguration
{
    private readonly ConcurrentDictionary<string, ScheduledTask> _tasks;
    private Action<Exception>? _errorHandler;
    private ILogger<IScheduler>? _logger;
    private readonly IMutex _mutex;
    private readonly int EventLockTimeout_24Hours = 1440;
    private readonly IServiceScopeFactory _scopeFactory;
    private int _schedulerIterationsActiveCount = 0;
    private readonly IDispatcher? _dispatcher;
    private string _currentWorkerName;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private bool _isFirstTick = true;

    public Scheduler(IMutex mutex, IServiceScopeFactory scopeFactory, IDispatcher? dispatcher)
    {
        _tasks = new ConcurrentDictionary<string, ScheduledTask>();
        _mutex = mutex;
        _scopeFactory = scopeFactory;
        _dispatcher = dispatcher;
        _currentWorkerName = "_default";
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void CancelAllCancellableTasks()
    {
        if (!_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
        }
    }


    public IScheduleInterval ScheduleAsync(Func<Task> asyncTaskToSchedule)
    {
        ScheduledEvent scheduled = new(asyncTaskToSchedule, _scopeFactory);
        _tasks.TryAdd(scheduled.OverlappingUniqueIdentifier(), new ScheduledTask(_currentWorkerName, scheduled));
        return scheduled;
    }

    public IScheduleInterval Schedule(Action actionToSchedule)
    {
        ScheduledEvent scheduled = new(actionToSchedule, _scopeFactory);
        _tasks.TryAdd(scheduled.OverlappingUniqueIdentifier(), new ScheduledTask(_currentWorkerName, scheduled));
        return scheduled;
    }

    public IScheduleInterval Schedule<T>() where T : IInvocable
    {
        ScheduledEvent scheduled = ScheduledEvent.WithInvocable<T>(_scopeFactory);
        _tasks.TryAdd(scheduled.OverlappingUniqueIdentifier(), new ScheduledTask(_currentWorkerName, scheduled));
        return scheduled;
    }

    public IScheduleInterval ScheduleWithParams<T>(params object[] parameters) where T : IInvocable
    {
        ScheduledEvent scheduled = ScheduledEvent.WithInvocableAndParams<T>(_scopeFactory, parameters);
        _tasks.TryAdd(scheduled.OverlappingUniqueIdentifier(), new ScheduledTask(_currentWorkerName, scheduled));
        return scheduled;
    }

    public IScheduleInterval ScheduleWithParams(Type invocableType, params object[] parameters)
    {
        ScheduledEvent scheduled = ScheduledEvent.WithInvocableAndParams(invocableType, _scopeFactory, parameters);
        _tasks.TryAdd(scheduled.OverlappingUniqueIdentifier(), new ScheduledTask(_currentWorkerName, scheduled));
        return scheduled;
    }

    public IScheduleInterval ScheduleInvocableType(Type invocableType)
    {
        if (!typeof(IInvocable).IsAssignableFrom(invocableType))
        {
            throw new ScheduleInvocableTypeException("ScheduleInvocableType must be passed in a type that implements IInvocable.");
        }

        ScheduledEvent scheduled = ScheduledEvent.WithInvocableType(invocableType, _scopeFactory);
        _tasks.TryAdd(scheduled.OverlappingUniqueIdentifier(), new ScheduledTask(_currentWorkerName, scheduled));
        return scheduled;
    }

    public IScheduler OnWorker(string workerName)
    {
        _currentWorkerName = workerName;
        return this;
    }

    public async Task RunSchedulerAsync()
    {
        DateTime utcNow = DateTime.UtcNow;
        await RunAtAsync(utcNow);
    }

    public async Task RunAtAsync(DateTime utcDate)
    {
        Interlocked.Increment(ref _schedulerIterationsActiveCount);
        bool isFirstTick = _isFirstTick;
        _isFirstTick = false;
        await RunWorkersAt(utcDate, isFirstTick);
        Interlocked.Decrement(ref _schedulerIterationsActiveCount);
    }

    public ISchedulerConfiguration OnError(Action<Exception> onError)
    {
        _errorHandler = onError;
        return this;
    }

    public ISchedulerConfiguration LogScheduledTaskProgress(ILogger<IScheduler> logger)
    {
        _logger = logger;
        return this;
    }

    public bool IsRunning => _schedulerIterationsActiveCount > 0;

    public bool TryUnschedule(string? uniqueIdentifier)
    {
        var toUnSchedule = _tasks.FirstOrDefault(scheduledEvent => scheduledEvent.Value.ScheduledEvent.OverlappingUniqueIdentifier() == uniqueIdentifier);

        if (toUnSchedule.Value != null)
        {
            var guid = toUnSchedule.Key;
            return _tasks.TryRemove(guid, out var dummy); // If failed, caller can try again etc.
        }

        return true; // Nothing to remove - was successful.
    }

    private async Task InvokeEventWithLoggerScope(ScheduledEvent scheduledEvent)
    {
        var eventInvocableTypeName = scheduledEvent.InvocableType()?.Name;
        using (_logger != null && eventInvocableTypeName != null ?
            _logger.BeginScope($"Invocable Type : {eventInvocableTypeName}") : null)
        {
            await InvokeEvent(scheduledEvent);
        }
    }

    private async Task InvokeEvent(ScheduledEvent scheduledEvent)
    {
        try
        {
            await TryDispatchEvent(new ScheduledEventStarted(scheduledEvent));

            async Task Invoke()
            {
                if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Scheduled task started...");
                }

                await scheduledEvent.InvokeScheduledEvent(_cancellationTokenSource.Token);

                if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Scheduled task finished...");
                }
            }

            if (scheduledEvent.ShouldPreventOverlapping())
            {
                if (_mutex.TryGetLock(scheduledEvent.OverlappingUniqueIdentifier(), EventLockTimeout_24Hours))
                {
                    try
                    {
                        await Invoke();
                    }
                    finally
                    {
                        _mutex.Release(scheduledEvent.OverlappingUniqueIdentifier());
                    }
                }
            }
            else
            {
                await Invoke();
            }

            await TryDispatchEvent(new ScheduledEventEnded(scheduledEvent));
        }
        catch (Exception e)
        {
            await TryDispatchEvent(new ScheduledEventFailed(scheduledEvent, e));

            if (_logger != null && _logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(e, "A scheduled task threw an Exception: ");
            }

            _errorHandler?.Invoke(e);
        }
    }

    /// <summary>
    /// This will grab all the scheduled tasks and combine each task into it's assigned "worker".
    /// Each worker runs on it's own thread and will process it's assigned scheduled tasks asynchronously.
    /// This method return a list of active tasks (one per worker - which needs to be awaited).
    /// </summary>
    /// <param name="utcDate"></param>
    /// <param name="isFirstTick"></param>
    /// <returns></returns>
    private async Task RunWorkersAt(DateTime utcDate, bool isFirstTick)
    {
        // Grab all the scheduled tasks so we can re-arrange them etc.
        var scheduledWorkers = new List<ScheduledTask>();

        foreach (var keyValue in _tasks)
        {
            var timerIsAtMinute = utcDate.Second == 0;
            var taskIsSecondsBased = !keyValue.Value.ScheduledEvent.IsScheduledCronBasedTask();
            var forceRunAtStart = isFirstTick && keyValue.Value.ScheduledEvent.ShouldRunOnceAtStart();
            var canRunBasedOnTimeMarker = taskIsSecondsBased || timerIsAtMinute;

            // If this task is scheduled as a cron based task (should only be checked if due per min)
            // but the time is not at the minute mark, we won't include those tasks to be checked if due.
            // The second based schedules are always checked.

            if (canRunBasedOnTimeMarker && keyValue.Value.ScheduledEvent.IsDue(utcDate))
            {
                scheduledWorkers.Add(keyValue.Value);
            }
            else if (forceRunAtStart)
            {
                scheduledWorkers.Add(keyValue.Value);
            }
        }

        // We want each "worker" (indicated by the "WorkerName" prop) to run on it's own thread.
        // So we'll group all the "due" scheduled events (the actual work the user wants to perform) into
        // buckets for each "worker".
        var groupedScheduledEvents = scheduledWorkers
            .GroupBy(worker => worker.WorkerName);

        var activeTasks = groupedScheduledEvents.Select(workerWithTasks =>
        {
            // Each group represents the "worker" for that group of scheduled events.
            // Running them on a separate thread means we can segment longer running tasks
            // onto their own thread, or maybe more cpu intensive operations onto an isolated thread, etc.
            return Task.Run(async () =>
            {
                foreach (var workerTask in workerWithTasks)
                {
                    var scheduledEvent = workerTask.ScheduledEvent;
                    await InvokeEventWithLoggerScope(scheduledEvent);
                }
            });
        });

        await Task.WhenAll(activeTasks);
    }

    private async Task TryDispatchEvent(IEvent toBroadcast)
    {
        if (_dispatcher != null)
        {
            await _dispatcher.Broadcast(toBroadcast);
        }
    }

    private sealed class ScheduledTask
    {
        public ScheduledTask(string workerName, ScheduledEvent scheduledEvent)
        {
            WorkerName = workerName;
            ScheduledEvent = scheduledEvent;
        }

        public ScheduledEvent ScheduledEvent { get; set; }
        public string WorkerName { get; set; }
    }
}
