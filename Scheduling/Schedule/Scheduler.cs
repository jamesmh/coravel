using System;
using System.Collections.Generic;

namespace Scheduling.Schedule
{
    public class Scheduler
    {
        private List<ScheduledEvent> _events;

        public Scheduler(){
            this._events = new List<ScheduledEvent>();
        }

        public IScheduled Schedule(Action actionToSchedule)
        {
            ScheduledEvent scheduled = new ScheduledEvent(actionToSchedule);
            this._events.Add(scheduled);
            return scheduled;
        }

        internal void RunTasksAtUtc(DateTime utcNow)
        {
            foreach(var scheduledEvent in this._events){
                if(scheduledEvent.ShouldInvokeNow(utcNow)){
                    scheduledEvent.InvokeScheduledAction();
                }
            }
        }
    }
}