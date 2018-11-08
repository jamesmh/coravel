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
        private ConcurrentDictionary<string, ScheduledEvent> _tasks;
        private Action<Exception> _errorHandler;
        private ILogger<IScheduler> _logger;
        private IMutex _mutex;
        private readonly int EventLockTimeout_24Hours = 1440;
        private IServiceScopeFactory _scopeFactory;
        private int _runningTasksCount = 0;
        private IDispatcher _dispatcher;

        public Scheduler(IMutex mutex, IServiceScopeFactory scopeFactory, IDispatcher dispatcher)
        {
            this._tasks = new ConcurrentDictionary<string, ScheduledEvent>();
            this._mutex = mutex;
            this._scopeFactory = scopeFactory;
            this._dispatcher = dispatcher;
        }

        public IScheduleInterval Schedule(Action actionToSchedule)
        {
            var scheduled = new ScheduledEvent(actionToSchedule);
            this._tasks.TryAdd(Guid.NewGuid().ToString(), scheduled);
            return scheduled;
        }

        public IScheduleInterval ScheduleAsync(Func<Task> asyncTaskToSchedule)
        {
            var scheduled = new ScheduledEvent(asyncTaskToSchedule);
            this._tasks.TryAdd(Guid.NewGuid().ToString(), scheduled);
            return scheduled;
        }

        public IScheduleInterval Schedule<T>() where T : IInvocable
        {
            var scheduled = ScheduledEvent.WithInvocable<T>(this._scopeFactory);
            this._tasks.TryAdd(Guid.NewGuid().ToString(), scheduled);
            return scheduled;
        }    

        public IScheduleInterval ScheduleInvocableType(Type invocableType)
        {
            if(!typeof(IInvocable).IsAssignableFrom(invocableType)){
                throw new Exception("ScheduleInvocableType must be passed in a type that implements IInvocable.");
            }

            var scheduled = ScheduledEvent.WithInvocableType(this._scopeFactory, invocableType);
            this._tasks.TryAdd(Guid.NewGuid().ToString(), scheduled);
            return scheduled;
        }        

        public async Task RunSchedulerAsync()
        {
            var utcNow = DateTime.UtcNow;
            await this.RunAtAsync(utcNow);
        }

        public async Task RunAtAsync(DateTime utcDate)
        {
            Interlocked.Increment(ref this._runningTasksCount);

            var activeTasks = new List<Task>();
            foreach (var keyValue in this._tasks)
            {
                var scheduledEvent = keyValue.Value;

                if (scheduledEvent.IsDue(utcDate))
                {
                    activeTasks.Add(InvokeEvent(scheduledEvent));
                }
            }

            await Task.WhenAll(activeTasks);

            Interlocked.Decrement(ref this._runningTasksCount);
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

        public bool IsRunning => this._runningTasksCount > 0;

        public bool TryUnschedule(string uniqueIndentifier) {
            var toUnschedule = this._tasks.First(scheduledEvent => scheduledEvent.Value.OverlappingUniqueIdentifier() == uniqueIndentifier);
            
            if(toUnschedule.Value != null)
            {
                var guid = toUnschedule.Key;
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
                }

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

                if (this._errorHandler is null)
                {
                    this._errorHandler(e);
                }
            }
        }

        private async Task TryDispatchEvent(IEvent toBroadcast)
        {
            if (this._dispatcher != null)
            {
                await this._dispatcher.Broadcast(toBroadcast);
            }
        }
    }
}