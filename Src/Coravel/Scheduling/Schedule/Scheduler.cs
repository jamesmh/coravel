using System;
using System.Collections.Generic;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule
{
    public class Scheduler : IScheduler, IHostedScheduler
    {
        private List<ScheduledEvent> _events;
        private Action<Exception> _errorHandler;
        private Queue _queue;

        public Scheduler()
        {
            this._events = new List<ScheduledEvent>();
        }

        public IScheduleInterval Schedule(Action actionToSchedule)
        {
            ScheduledEvent scheduled = new ScheduledEvent(actionToSchedule);
            this._events.Add(scheduled);
            return scheduled;
        }

        public void RunScheduler()
        {
            DateTime utcNow = DateTime.UtcNow;
            this.RunAt(utcNow);
        }

        public void RunAt(DateTime utcDate) {
            // Minutes is lowest value used in scheduling calculations
            utcDate = new DateTime(utcDate.Year, utcDate.Month, utcDate.Day, utcDate.Hour, utcDate.Minute, 0);

            ConsumeQueuedTasks();
            InvokeScheduledTasks(utcDate);
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

        private void InvokeScheduledTasks(DateTime utcNow)
        {
            foreach (var scheduledEvent in this._events)
            {
                if (scheduledEvent.ShouldInvokeNow(utcNow))
                {
                    this.InvokeActionWithErrorHandling(scheduledEvent.InvokeScheduledAction);
                }
            }
        }

        private void InvokeActionWithErrorHandling(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (this._errorHandler == null)
                {
                    throw e;
                }
                else
                {
                    this._errorHandler(e);
                }
            }
        }
    }
}