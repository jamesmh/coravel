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

namespace Coravel.Scheduling.Schedule
{
    public class Scheduler : IScheduler, ISchedulerConfiguration, IDisposable
    {
        private ConcurrentBag<ScheduledEvent> _tasks;
        private Action<Exception> _errorHandler;
        private ILogger<IScheduler> _logger;
        private IMutex _mutex;
        private readonly int EventLockTimeout_24Hours = 1440;
        private IServiceScopeFactory _scopeFactory;

        public Scheduler(IMutex mutex, IServiceScopeFactory scopeFactory)
        {
            this._tasks = new ConcurrentBag<ScheduledEvent>();
            this._mutex = mutex;
            this._scopeFactory = scopeFactory;
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
            DateTime utcNow = DateTime.UtcNow;
            await this.RunAtAsync(utcNow);
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

        public void Dispose()
        {
            this.RunSchedulerAsync().GetAwaiter().GetResult();
        }

        private async Task InvokeEvent(ScheduledEvent scheduledEvent)
        {
            try
            {
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
                        await Invoke();
                    }
                }
                else
                {
                    await Invoke();
                }

            }
            catch (Exception e)
            {
                this._logger?.LogError("A scheduled task threw an Exception: " + e.Message);
                if (this._errorHandler != null)
                {
                    this._errorHandler(e);
                }
            }
        }
    }
}