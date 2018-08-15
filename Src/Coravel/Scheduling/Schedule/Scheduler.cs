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

namespace Coravel.Scheduling.Schedule
{
    public class Scheduler : IScheduler, ISchedulerConfiguration, IDisposable
    {
        private ConcurrentBag<ScheduledEvent> _tasks;
        private Action<Exception> _errorHandler;
        private ILogger<IScheduler> _logger;

        public Scheduler()
        {
            this._tasks = new ConcurrentBag<ScheduledEvent>();
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
                    activeTasks.Add(InvokeTask(scheduledEvent));
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

        private async Task InvokeTask(ScheduledEvent scheduledEvent)
        {
            try
            {
                this._logger?.LogInformation("Scheduled task started...");
                await scheduledEvent.InvokeScheduledEvent();
                this._logger?.LogInformation("Scheduled task finished...");
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