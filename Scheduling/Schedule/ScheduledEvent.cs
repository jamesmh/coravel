using System;

namespace Scheduling.Schedule
{
    public class ScheduledEvent : IScheduled
    {
        private TimeSpan _scheduledInterval;
        private DateTime _utcLastRun;
        private Action _scheduledAction;

        public ScheduledEvent(Action scheduledAction) {
            this._scheduledAction = scheduledAction;
        }

        internal bool ShouldInvokeNow(DateTime utcNow)
        {
            if (IntervalSinceLstRun(utcNow) >= this._scheduledInterval)
            {
                this._utcLastRun = utcNow;
                return true;
            }
            return false;
        }

        private TimeSpan IntervalSinceLstRun(DateTime utcNow) =>
            utcNow.AddSeconds(1).Subtract(this._utcLastRun);
        

        internal void InvokeScheduledAction() => this._scheduledAction();

        public void Daily() => this._scheduledInterval = TimeSpan.FromDays(1);

        public void EveryHour() => this._scheduledInterval = TimeSpan.FromHours(1);

        public void EveryMinute() => this._scheduledInterval = TimeSpan.FromMinutes(1);

        public void EachMinutes(int minutes)=> this._scheduledInterval = TimeSpan.FromMinutes(minutes);
    }
}