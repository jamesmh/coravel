using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule
{
    public class Scheduler : IScheduler, IHostedScheduler, IDisposable
    {
        private List<IScheduledTask> _events;
        private Action<Exception> _errorHandler;
        private Queue _queue;

        public Scheduler()
        {
            this._events = new List<IScheduledTask>();
        }

        public IScheduleInterval Schedule(Action actionToSchedule)
        {
            ScheduledTask scheduled = new ScheduledTask(actionToSchedule);
            this._events.Add(scheduled);
            return scheduled;
        }

        public IScheduleInterval ScheduleAsync(Func<Task> asyncTaskToSchedule)
        {
            ScheduledAsyncTask scheduled = new ScheduledAsyncTask(asyncTaskToSchedule);
            this._events.Add(scheduled);
            return scheduled;
        }

        public void RunScheduler()
        {
            DateTime utcNow = DateTime.UtcNow;
            this.RunAt(utcNow);
        }

        public void RunAt(DateTime utcDate)
        {
            // Minutes is lowest value used in scheduling calculations
            utcDate = new DateTime(utcDate.Year, utcDate.Month, utcDate.Day, utcDate.Hour, utcDate.Minute, 0);

            ConsumeQueuedTasks();
            InvokeScheduledTasksAsync(utcDate);
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
            this.RunScheduler();
        }

        private void ConsumeQueuedTasks()
        {
            if (this._queue == null)
                return;

            IEnumerable<Action> queuedTasks = this._queue.DequeueAllTasks();

            foreach (Action task in queuedTasks)
            {
                this.InvokeActionWithErrorHandling(task);
            }
        }

        private async Task InvokeScheduledTasksAsync(DateTime utcNow)
        {
            var tasks = this._events
                .Where(e => e is ScheduledTask)
                .Select(e => e as ScheduledTask);

            var asyncTasks = this._events
                .Where(e => e is ScheduledAsyncTask)
                .Select(e => e as ScheduledAsyncTask);

            foreach (var scheduledEvent in tasks)
            {
                if (scheduledEvent.ShouldInvokeNow(utcNow))
                {
                    this.InvokeActionWithErrorHandling(scheduledEvent.InvokeScheduledAction);
                }
            }

            foreach (var asyncTask in asyncTasks)
            {
                if (asyncTask.ShouldInvokeNow(utcNow))
                {
                    await this.InvokeActionWithErrorHandlingAsync(asyncTask.InvokeAsync);
                }
            }
        }

        //TODO: Move error handling into the task Invoke. Pass in the scheduler error handler as param.
        private void InvokeActionWithErrorHandling(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (this._errorHandler != null)
                {
                    this._errorHandler(e);
                }
            }
        }

        private async Task InvokeActionWithErrorHandlingAsync(Func<Task> task)
        {
            try
            {
                await task();
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