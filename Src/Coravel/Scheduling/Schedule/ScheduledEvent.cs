using System;
using System.Collections.Generic;
using System.Linq;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Scheduling.Schedule.Restrictions;

namespace Coravel.Scheduling.Schedule
{
    public class ScheduledEvent : IScheduleInterval
    {
        private TimeSpan _scheduledInterval;
        private DateTime _utcLastRun;
        private Action _scheduledAction;
        private DayRestrictions _dayRestrictions;
        private TimeRestrictions _timeRestrictions;


        public ScheduledEvent(Action scheduledAction)
        {
            this._scheduledAction = scheduledAction;
            this._dayRestrictions = new DayRestrictions();
            this._timeRestrictions = new TimeRestrictions();
        }

        public bool ShouldInvokeNow(DateTime utcNow)
        {
            bool scheduledNow = IntervalSinceLastRun(utcNow) >= this._scheduledInterval;
            bool restrictionsPassed = PassesRestrictions(utcNow);

            if (scheduledNow && restrictionsPassed)
            {
                this._utcLastRun = utcNow;
                return true;
            }
            return false;
        }

        public void InvokeScheduledAction() => this._scheduledAction();

        public IScheduleRestriction Daily()
        {
            this._scheduledInterval = TimeSpan.FromDays(1);
            return this._dayRestrictions;
        }

        public IScheduleRestriction DailyAtHour(int hour)
        {
            this._timeRestrictions.OccursAtHour(hour);
            return this.Daily();
        }

        public IScheduleRestriction DailyAt(int hour, int minute)
        {
            this._timeRestrictions.OccursAt(hour, minute);
            return this.Daily();
        }

        public IScheduleRestriction Hourly()
        {
            this._scheduledInterval = TimeSpan.FromHours(1);
            return this._dayRestrictions;
        }

        public IScheduleRestriction HourlyAt(int minute)
        {
            this._timeRestrictions.OccursAtMinute(minute);
            return this.Hourly();
        }

        public IScheduleRestriction EveryMinute()
        {
            this.AfterMinutes(1);
            return this._dayRestrictions;
        }

        public IScheduleRestriction EveryFiveMinutes()
        {
            this.AfterMinutes(5);
            return this._dayRestrictions;
        }

        public IScheduleRestriction EveryTenMinutes()
        {
            this.AfterMinutes(10);
            return this._dayRestrictions;
        }

        public IScheduleRestriction EveryFifteenMinutes()
        {
            this.AfterMinutes(15);
            return this._dayRestrictions;
        }

        public IScheduleRestriction EveryThirtyMinutes()
        {
            this.AfterMinutes(30);
            return this._dayRestrictions;
        }

        public IScheduleRestriction Weekly()
        {
            this._scheduledInterval = TimeSpan.FromDays(7);
            return this._dayRestrictions;
        }

        private TimeSpan IntervalSinceLastRun(DateTime utcNow) =>
            utcNow.Subtract(this._utcLastRun);

        private IScheduleRestriction AfterMinutes(int minutes)
        {
            this._scheduledInterval = TimeSpan.FromMinutes(minutes);
            return this._dayRestrictions;
        }

        private bool PassesRestrictions(DateTime utcNow) =>
            this._dayRestrictions.PassesRestrictions(utcNow)
            && this._timeRestrictions.PassesRestrictions(utcNow);
    }
}