using System;
using System.Collections.Generic;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule
{
    public class Scheduler : IScheduler, IHostedScheduler
    {
        private List<ScheduledEvent> _events;
        private Action<Exception> _errorHandler;

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

        public void RunScheduledTasks(DateTime utcNow)
        {
            foreach (var scheduledEvent in this._events)
            {
                if (scheduledEvent.ShouldInvokeNow(utcNow))
                {
                    try
                    {
                        scheduledEvent.InvokeScheduledAction();
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

        public IHostedScheduler OnError(Action<Exception> onError)
        {
            this._errorHandler = onError;
            return this;
        }
    }
}