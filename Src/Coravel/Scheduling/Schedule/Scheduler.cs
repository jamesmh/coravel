using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Scheduling.Schedule.Tasks;

namespace Coravel.Scheduling.Schedule
{
    public class Scheduler : IScheduler, IHostedScheduler, IDisposable
    {
        private List<ScheduledTask> _tasks;
        private Action<Exception> _errorHandler;
        private Queue _queue;

        public Scheduler()
        {
            this._tasks = new List<ScheduledTask>();
        }

        public IScheduleInterval Schedule(Action actionToSchedule)
        {
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
            await this.RunAt(utcNow);
        }

        public async Task RunAt(DateTime utcDate)
        {
            // Minutes is lowest value used in scheduling calculations
            utcDate = new DateTime(utcDate.Year, utcDate.Month, utcDate.Day, utcDate.Hour, utcDate.Minute, 0);

            ConsumeQueuedTasks();
            await InvokeScheduledTasksAsync(utcDate);
        }

        public IHostedScheduler OnError(Action<Exception> onError)
        {
            this._errorHandler = onError;
            return this;
        }

        public IQueue UseQueue()
        {
            if (this._queue == null)
            {
                this._queue = new Queue();
            }
            return this._queue;
        }

        public void Dispose()
        {
            this.RunSchedulerAsync().GetAwaiter().GetResult();
        }

        private void ConsumeQueuedTasks()
        {
            if (this._queue == null)
                return;

            IEnumerable<Action> queuedTasks = this._queue.DequeueAllTasks();

            foreach (Action task in queuedTasks)
            {
                try
                {
                    task();
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

        private async Task InvokeScheduledTasksAsync(DateTime utcNow)
        {
            foreach (var scheduledEvent in this._tasks)
            {
                if (scheduledEvent.ShouldInvokeNow(utcNow))
                {
                    try
                    {
                        await scheduledEvent.InvokeScheduledAction();
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
        }
    }
}