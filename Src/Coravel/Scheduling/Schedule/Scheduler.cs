using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Helpers;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Tasks;
using Microsoft.Extensions.Logging;

namespace Coravel.Scheduling.Schedule
{
    public class Scheduler : IScheduler, ISchedulerConfiguration, IDisposable
    {
        private List<ScheduledTask> _tasks;
        private Action<Exception> _errorHandler;
        private ILogger<IScheduler> _logger;

        public Scheduler()
        {
            this._tasks = new List<ScheduledTask>();
        }

        public IScheduleInterval Schedule(Action actionToSchedule)
        {
            if (actionToSchedule.IsThisAsync()){
                System.Diagnostics.Debug.WriteLine($"Action is async but it will be called synchronously. " +
                                  "You could use ScheduleAsync method to have it run asynchronously");
            }

            ScheduledTask scheduled = new ScheduledTask(actionToSchedule);
            this._tasks.Add(scheduled);
            return scheduled;
        }

        public IScheduleInterval ScheduleAsync(Func<Task> asyncTaskToSchedule)
        {
            ScheduledTask scheduled = new ScheduledTask(asyncTaskToSchedule);
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
            // Minutes is lowest value used in scheduling calculations
            utcDate = new DateTime(utcDate.Year, utcDate.Month, utcDate.Day, utcDate.Hour, utcDate.Minute, 0);
            await InvokeScheduledTasksAsync(utcDate);
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

        private async Task InvokeScheduledTasksAsync(DateTime utcNow)
        {            
            foreach (var scheduledEvent in this._tasks)
            {
                if (scheduledEvent.ShouldInvokeNow(utcNow))
                {
                    try
                    {
                        this._logger?.LogInformation("Scheduled task started...");
                        await scheduledEvent.InvokeScheduledAction();
                        this._logger?.LogInformation("Scheduled task finished...");
                    }
                    catch (Exception e)
                    {
                        this._logger?.LogWarning("A scheduled task threw an Exception: " + e.Message);
                        if (this._errorHandler != null)
                        {
                            this._errorHandler(e);
                        }
                    }
                }
            }
        }
    }
}