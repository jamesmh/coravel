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

namespace Coravel.Scheduling.Schedule
{
    public class Scheduler : IScheduler, ISchedulerConfiguration
    {
        private ConcurrentBag<ScheduledEvent> _tasks;
        private Action<Exception> _errorHandler;
        private ILogger<IScheduler> _logger;
        private IMutex _mutex;
        private readonly int EventLockTimeout_24Hours = 1440;
        private IServiceScopeFactory _scopeFactory;
        private bool _isRunning = false;
        private IDispatcher _dispatcher;

        public Scheduler(IMutex mutex, IServiceScopeFactory scopeFactory, IDispatcher dispatcher)
        {
            this._tasks = new ConcurrentBag<ScheduledEvent>();
            this._mutex = mutex;
            this._scopeFactory = scopeFactory;
            this._dispatcher = dispatcher;
        }

        public IScheduleInterval Schedule(Action actionToSchedule)
        {
            ScheduledEvent scheduled = new ScheduledEvent(actionToSchedule);
            this._tasks.Add(scheduled);
            return scheduled;
        }

        public IScheduleInterval ScheduleAsync(Func<Task> asyncTaskToSchedule)
        {
            ScheduledEvent scheduled = new ScheduledEvent(asyncTaskToSchedule);
            this._tasks.Add(scheduled);
            return scheduled;
        }

        public IScheduleInterval Schedule<T>() where T : IInvocable
        {
            ScheduledEvent scheduled = ScheduledEvent.WithInvocable<T>(this._scopeFactory);
            this._tasks.Add(scheduled);
            return scheduled;
        }

        public async Task RunSchedulerAsync()
        {
            await this.MarkSchedulerAsRunning(async () =>
            {
                DateTime utcNow = DateTime.UtcNow;
                await this.RunAtAsync(utcNow);
            });
        }

        public async Task RunAtAsync(DateTime utcDate)
        {
            var activeTasks = new List<Task>();
            foreach (var scheduledEvent in this._tasks)
            {
                if (scheduledEvent.IsDue(utcDate))
                {
                    activeTasks.Add(InvokeEvent(scheduledEvent));
                }
            }

            await Task.WhenAll(activeTasks);
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

        public bool IsStillRunning() => this._isRunning; // Will be read from another thread. There will only be one writer.

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
            finally
            {                
                await this.TryDispatchEvent(new ScheduledEventEnded(scheduledEvent));                
            }
        }

        private async Task MarkSchedulerAsRunning(Func<Task> func)
        {
            this._isRunning = true;
            await func();
            this._isRunning = false;
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