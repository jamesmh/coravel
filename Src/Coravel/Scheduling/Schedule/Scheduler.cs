using System;
using System.Collections.Generic;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule
{
    public class Scheduler : IScheduler
    {
        private List<ScheduledEvent> _events;

        public Scheduler(){
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
            foreach(var scheduledEvent in this._events){
                if(scheduledEvent.ShouldInvokeNow(utcNow)){
                    scheduledEvent.InvokeScheduledAction();
                }
            }
        }
    }
}