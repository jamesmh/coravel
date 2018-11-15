using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Coravel.Scheduling.Schedule.Helpers;
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
        private ILogger<IScheduler> _logger;
        private IMutex _mutex;
        private readonly int EventLockTimeout_24Hours = 1440;
        private IServiceScopeFactory _scopeFactory;
        private int _schedulerIterationsActiveCount = 0;
        private IDispatcher _dispatcher;
        private string _currentWorkerName;

        public Scheduler(IMutex mutex, IServiceScopeFactory scopeFactory, IDispatcher dispatcher)
        {
            this._tasks = new ConcurrentDictionary<string, ScheduledTask>();
            this._mutex = mutex;
            this._scopeFactory = scopeFactory;
            this._dispatcher = dispatcher;
            this._currentWorkerName = "_default";
        }

        public IScheduleInterval Schedule(Action actionToSchedule)
        {
            ScheduledEvent scheduled = new ScheduledEvent(actionToSchedule);
            this._tasks.TryAdd(Guid.NewGuid().ToString(), new ScheduledTask(this._currentWorkerName, scheduled));
            return scheduled;
        }

        public IScheduleInterval ScheduleAsync(Func<Task> asyncTaskToSchedule)
        {
            ScheduledEvent scheduled = new ScheduledEvent(asyncTaskToSchedule);
            this._tasks.TryAdd(Guid.NewGuid().ToString(), new ScheduledTask(this._currentWorkerName, scheduled));
            return scheduled;
        }

        public IScheduleInterval Schedule<T>() where T : IInvocable
        {
            ScheduledEvent scheduled = ScheduledEvent.WithInvocable<T>(this._scopeFactory);
            this._tasks.TryAdd(Guid.NewGuid().ToString(), new ScheduledTask(this._currentWorkerName, scheduled));
            return scheduled;
        }

        public IScheduleInterval ScheduleInvocableType(Type invocableType)
        {
            if (!typeof(IInvocable).IsAssignableFrom(invocableType))
            {
                throw new Exception("ScheduleInvocableType must be passed in a type that implements IInvocable.");
            }

            ScheduledEvent scheduled = ScheduledEvent.WithInvocableType(this._scopeFactory, invocableType);
            this._tasks.TryAdd(Guid.NewGuid().ToString(), new ScheduledTask(this._currentWorkerName, scheduled));
            return scheduled;
        }

        public IScheduler OnWorker(string workerName)
        {
            this._currentWorkerName = workerName;
            return this;
        }

        public async Task RunSchedulerAsync()
        {
            DateTime utcNow = DateTime.UtcNow;
            await this.RunAtAsync(utcNow);
        }

        public async Task RunAtAsync(DateTime utcDate)
        {
            Interlocked.Increment(ref this._schedulerIterationsActiveCount);
            await RunWorkersAt(utcDate);
            Interlocked.Decrement(ref this._schedulerIterationsActiveCount);
        }

        public ISchedulerConfiguration OnError(Action<Exception> onError)
        {
            this._errorHandler = onError;
            return this;
        }

        public ISchedulerConfiguration LogScheduledTaskProgress(ILogger<IScheduler> logger)
        {
            this._logger = logger;
            return this;
        }

        public bool IsRunning => this._schedulerIterationsActiveCount > 0;

        public bool TryUnschedule(string uniqueIndentifier)
        {
            var toUnschedule = this._tasks.First(scheduledEvent => scheduledEvent.Value.ScheduledEvent.OverlappingUniqueIdentifier() == uniqueIndentifier);

            if (toUnschedule.Value != null)
            {
                string guid = toUnschedule.Key;
                return this._tasks.TryRemove(guid, out var dummy); // If failed, caller can try again etc.
            }

            return true; // Nothing to remove - was successful.
        }

        private async Task InvokeEvent(ScheduledEvent scheduledEvent)
        {
            try
            {
                await this.TryDispatchEvent(new ScheduledEventStarted(scheduledEvent));

                async Task Invoke()
                {
                    this._logger?.LogInformation("Scheduled task started...");
                    await scheduledEvent.InvokeScheduledEvent();
                    this._logger?.LogInformation("Scheduled task finished...");
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

                this._logger?.LogError("A scheduled task threw an Exception: " + e.Message);

                if (this._errorHandler != null)
                {
                    this._errorHandler(e);
                }
            }
        }

        /// <summary>
        /// This will grab all the scheduled tasks and combine each task into it's assigned "worker".
        /// Each worker runs on it's own thread and will process it's assigned scheduled tasks asynchronously.
        /// This method return a list of active tasks (one per worker - which needs to be awaited).
        /// </summary>
        /// <param name="utcDate"></param>
        /// <returns></returns>
        private async Task RunWorkersAt(DateTime utcDate)
        {
            // Grab all the scheduled tasks so we can re-arrange them etc.
            List<ScheduledTask> scheduledWorkers = new List<ScheduledTask>();
            foreach (var keyValue in this._tasks)
            {
                scheduledWorkers.Add(keyValue.Value);
            }

            // We want each "worker" (indicated by the "WorkerName" prop) to run on it's own thread.
            // So we'll group all the "due" scheduled events (the actual work the user wants to perform) into
            // buckets for each "worker".
            var groupedScheduledEvents = scheduledWorkers
                .Where(worker => worker.ScheduledEvent.IsDue(utcDate))
                .GroupBy(worker => worker.WorkerName);

            var activeTasks = groupedScheduledEvents.Select(workerWithTasks => {
                // Each group represents the "worker" for that group of scheduled events.
                // Running them on a separate thread means we can segment longer running tasks
                // onto their own thread, or maybe more cpu intensive operations onto an isolated thread, etc.
                return Task.Run(async () =>
                {
                    foreach (var workerTask in workerWithTasks)
                    {
                        var scheduledEvent = workerTask.ScheduledEvent;
                        await InvokeEvent(scheduledEvent);                        
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

        private class ScheduledTask {
            public ScheduledTask(string workerName, ScheduledEvent scheduledEvent)
            {
                this.WorkerName = workerName;
                this.ScheduledEvent = scheduledEvent;
            }

            public ScheduledEvent ScheduledEvent {get;set;}
            public string WorkerName {get;set;}
        }
    }
}