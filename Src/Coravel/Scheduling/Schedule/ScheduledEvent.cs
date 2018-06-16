using System;
using System.Collections.Generic;
using System.Linq;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule
{
    public class ScheduledEvent : IScheduleInterval, IScheduleRestriction
    {
        private TimeSpan _scheduledInterval;
        private DateTime _utcLastRun;
        private Action _scheduledAction;
        private List<DayOfWeek> _restrictions;

        public ScheduledEvent(Action scheduledAction)
        {
            this._scheduledAction = scheduledAction;
            this._restrictions = new List<DayOfWeek>();
        }

        private TimeSpan IntervalSinceLstRun(DateTime utcNow) =>
            utcNow.AddSeconds(1).Subtract(this._utcLastRun);

        internal bool ShouldInvokeNow(DateTime utcNow)
        {
            bool scheduledNow = IntervalSinceLstRun(utcNow) >= this._scheduledInterval;
            bool restrictionsPassed = PassesRestrictions(utcNow);

            if(scheduledNow && restrictionsPassed)
            {
                this._utcLastRun = utcNow;
                return true;
            }
            return false;
        }

        private bool PassesRestrictions(DateTime utcNow) =>
            this._restrictions.Any()
                ? this._restrictions.Contains(utcNow.DayOfWeek)
                : true;        

        internal void InvokeScheduledAction() => this._scheduledAction();

        public IScheduleRestriction Daily()
        {
            this._scheduledInterval = TimeSpan.FromDays(1);
            return this;
        }

        public IScheduleRestriction Hourly()
        {
            this._scheduledInterval = TimeSpan.FromHours(1);
            return this;
        }

        public IScheduleRestriction EveryMinute()
        {
            this.AfterMinutes(1);
            return this;
        }

        public IScheduleRestriction AfterMinutes(int minutes)
        {
            this._scheduledInterval = TimeSpan.FromMinutes(minutes);
            return this;
        }

        public IScheduleRestriction EveryFiveMinutes()
        {
            this.AfterMinutes(5);
            return this;
        }

        public IScheduleRestriction EveryTenMinutes()
        {
            this.AfterMinutes(10);
            return this;
        }

        public IScheduleRestriction EveryFifteenMinutes()
        {
            this.AfterMinutes(15);
            return this;
        }

        public IScheduleRestriction EveryThirtyMinutes()
        {
            this.AfterMinutes(30);
            return this;
        }

        public IScheduleRestriction Weekly()
        {
            this._scheduledInterval = TimeSpan.FromDays(7);
            return this;
        }

        public IScheduleRestriction Monday()
        {
            this._restrictions.Add(DayOfWeek.Monday);
            return this;
        }

        public IScheduleRestriction Tuesday()
        {
            this._restrictions.Add(DayOfWeek.Tuesday);
            return this;
        }

        public IScheduleRestriction Wednesday()
        {
            this._restrictions.Add(DayOfWeek.Wednesday);
            return this;
        }

        public IScheduleRestriction Thursday()
        {
            this._restrictions.Add(DayOfWeek.Thursday);
            return this;
        }

        public IScheduleRestriction Friday()
        {
            this._restrictions.Add(DayOfWeek.Friday);
            return this;
        }

        public IScheduleRestriction Saturday()
        {
            this._restrictions.Add(DayOfWeek.Saturday);
            return this;
        }

        public IScheduleRestriction Sunday()
        {
            this._restrictions.Add(DayOfWeek.Sunday);
            return this;
        }

        public IScheduleRestriction Weekday()
        {
            this.Monday();
            this.Tuesday();
            this.Wednesday();
            this.Thursday();
            this.Friday();
            return this;
        }

        public IScheduleRestriction Weekend()
        {
            this.Saturday();
            this.Sunday();
            return this;
        }
    }
}