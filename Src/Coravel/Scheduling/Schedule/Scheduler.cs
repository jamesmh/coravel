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

namespace Coravel.Scheduling.Schedule
{
    public class Scheduler : IScheduler, ISchedulerConfiguration
    {
        private ConcurrentDictionary<string, ScheduledTask> _tasks;
        private Action<Exception> _errorHandler;
        private bool _shouldLogProgress = false;
        private IMutex _mutex;
        private readonly int EventLockTimeout_24Hours = 1440;
        private IServiceScopeFactory _scopeFactory;
        private int _schedulerIterationsActiveCount = 0;
        private IDispatcher _dispatcher;
        private string _currentWorkerName;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isFirstTick = true;
        private object _isFirstTickLockObj = new object();

        public Scheduler(IMutex mutex, IServiceScopeFactory scopeFactory, IDispatcher dispatcher)
        {
            this._tasks = new ConcurrentDictionary<string, ScheduledTask>();
            this._mutex = mutex;
            this._scopeFactory = scopeFactory;
            this._dispatcher = dispatcher;
            this._currentWorkerName = "_default";
            this._cancellationTokenSource = new CancellationTokenSource();
        }

        public void CancelAllCancellableTasks()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                this._cancellationTokenSource.Cancel();
            }
        }

        public IScheduleInterval Schedule(Action actionToSchedule)
        {
            ScheduledEvent scheduled = new ScheduledEvent(actionToSchedule, this._scopeFactory);
            this._tasks.TryAdd(scheduled.OverlappingUniqueIdentifier(), new ScheduledTask(this._currentWorkerName, scheduled));
            return scheduled;
        }

        public IScheduleInterval ScheduleAsync(Func<Task> asyncTaskToSchedule)
        {
            ScheduledEvent scheduled = new ScheduledEvent(asyncTaskToSchedule, this._scopeFactory);
            this._tasks.TryAdd(scheduled.OverlappingUniqueIdentifier(), new ScheduledTask(this._currentWorkerName, scheduled));
            return scheduled;
        }

        public IScheduleInterval Schedule<T>() where T : IInvocable
        {
            ScheduledEvent scheduled = ScheduledEvent.WithInvocable<T>(this._scopeFactory);
            this._tasks.TryAdd(scheduled.OverlappingUniqueIdentifier(), new ScheduledTask(this._currentWorkerName, scheduled));
            return scheduled;
        }

        public IScheduleInterval ScheduleWithParams<T>(params object[] parameters) where T : IInvocable
        {
            ScheduledEvent scheduled = ScheduledEvent.WithInvocableAndParams<T>(this._scopeFactory, parameters);
            this._tasks.TryAdd(scheduled.OverlappingUniqueIdentifier(), new ScheduledTask(this._currentWorkerName, scheduled));
            return scheduled;
        }

        public IScheduleInterval ScheduleWithParams(Type invocableType, params object[] parameters)
        {
            ScheduledEvent scheduled = ScheduledEvent.WithInvocableAndParams(invocableType, this._scopeFactory, parameters);
            this._tasks.TryAdd(scheduled.OverlappingUniqueIdentifier(), new ScheduledTask(this._currentWorkerName, scheduled));
            return scheduled;
        }

        public IScheduleInterval ScheduleInvocableType(Type invocableType)
        {
            if (!typeof(IInvocable).IsAssignableFrom(invocableType))
            {
                throw new Exception("ScheduleInvocableType must be passed in a type that implements IInvocable.");
            }

            ScheduledEvent scheduled = ScheduledEvent.WithInvocableType(invocableType, this._scopeFactory);
            this._tasks.TryAdd(scheduled.OverlappingUniqueIdentifier(), new ScheduledTask(this._currentWorkerName, scheduled));
            return scheduled;
        }

        public IScheduler OnWorker(string workerName)
        {
            this._currentWorkerName = workerName;
            return this;
        }

        public async Task RunAtAsync(DateTime utcDate)
        {
            Interlocked.Increment(ref this._schedulerIterationsActiveCount);
            
            // Possibility for race condition here.
            // Note: This was a bug fixed by issue 353.
            bool isFirstTick = false;
            if(this._isFirstTick)
            {
                lock(this._isFirstTickLockObj)
                {
                    // Set to `this._isFirstTick` not `true` because there might be multiple threads coming into
                    // this critical section - but only one should count as "the first run".
                    isFirstTick = this._isFirstTick;
                    this._isFirstTick = false;
                }
            }

            await RunWorkersAt(utcDate, isFirstTick);
            Interlocked.Decrement(ref this._schedulerIterationsActiveCount);
        }

        public ISchedulerConfiguration OnError(Action<Exception> onError)
        {
            this._errorHandler = onError;
            return this;
        }

        public ISchedulerConfiguration LogScheduledTaskProgress()
        {
            this._shouldLogProgress = true;
            return this;
        }

        public bool IsRunning => this._schedulerIterationsActiveCount > 0;

        public bool TryUnschedule(string uniqueIdentifier)
        {
            var toUnSchedule = this._tasks.FirstOrDefault(scheduledEvent => scheduledEvent.Value.ScheduledEvent.OverlappingUniqueIdentifier() == uniqueIdentifier);

            if (toUnSchedule.Value != null)
            {
                var guid = toUnSchedule.Key;
                return this._tasks.TryRemove(guid, out var dummy); // If failed, caller can try again etc.
            }

            return true; // Nothing to remove - was successful.
        }

        public IReadOnlyList<ScheduleInfo> GetSchedules()
        {
            return this._tasks.Values
                .Select(task => task.ScheduledEvent)
                .OfType<IGetAllScheduleInfo>()
                .Select(dataProvider => dataProvider.GetScheduleInfo())
                .ToList();
        }

        private async Task InvokeEventWithLoggerScope(ScheduledEvent scheduledEvent)
        {         
            using var scope = this._scopeFactory.CreateAsyncScope();
            ILogger<IScheduler> logger = null;

            if(this._shouldLogProgress)
            {
                logger = scope.ServiceProvider.GetRequiredService<ILogger<IScheduler>>();
            }

            var eventInvocableTypeName = scheduledEvent.InvocableType()?.Name;
            using (logger != null && eventInvocableTypeName != null ?
                logger.BeginScope($"Invocable Type : {eventInvocableTypeName}") : null)
            {
                await InvokeEvent(scheduledEvent, logger);
            }
        }

        private async Task InvokeEvent(ScheduledEvent scheduledEvent, ILogger<IScheduler> logger)
        {
            try
            {
                await this.TryDispatchEvent(new ScheduledEventStarted(scheduledEvent));

                async Task Invoke()
                {
                    logger?.LogDebug("Scheduled task started...");
                    await scheduledEvent.InvokeScheduledEvent(this._cancellationTokenSource.Token);
                    logger?.LogDebug("Scheduled task finished...");
                };

                if (scheduledEvent.ShouldPreventOverlapping())
                {
                    if (this._mutex.TryGetLock(scheduledEvent.OverlappingUniqueIdentifier(), EventLockTimeout_24Hours))
                    {
                        try
                        {
                            await Invoke();
                        }
                        finally
                        {
                            this._mutex.Release(scheduledEvent.OverlappingUniqueIdentifier());
                        }
                    }
                }
                else
                {
                    await Invoke();
                }

                await this.TryDispatchEvent(new ScheduledEventEnded(scheduledEvent));
            }
            catch (Exception e)
            {
                await this.TryDispatchEvent(new ScheduledEventFailed(scheduledEvent, e));

                logger?.LogError(e, "A scheduled task threw an Exception: ");

                this._errorHandler?.Invoke(e);
            }
        }

        /// <summary>
        /// This will grab all the scheduled tasks and combine each task into its assigned "worker".
        /// Each worker runs on its own thread and will process its assigned scheduled tasks asynchronously.
        /// This method return a list of active tasks (one per worker - which needs to be awaited).
        /// </summary>
        /// <param name="utcDate"></param>
        /// <param name="isFirstTick"></param>
        /// <returns></returns>
        private async Task RunWorkersAt(DateTime utcDate, bool isFirstTick)
        {
            // Grab all the scheduled tasks so we can re-arrange them etc.
            var scheduledWorkers = new List<ScheduledTask>();

            foreach (var keyValue in this._tasks)
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

            // We want each "worker" (indicated by the "WorkerName" prop) to run on its own thread.
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
            if (this._dispatcher != null)
            {
                await this._dispatcher.Broadcast(toBroadcast);
            }
        }

        private class ScheduledTask
        {
            public ScheduledTask(string workerName, ScheduledEvent scheduledEvent)
            {
                this.WorkerName = workerName;
                this.ScheduledEvent = scheduledEvent;
            }

            public ScheduledEvent ScheduledEvent { get; set; }
            public string WorkerName { get; set; }
        }
    }
}
