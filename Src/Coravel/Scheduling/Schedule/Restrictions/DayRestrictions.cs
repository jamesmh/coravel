using System;
using System.Collections.Generic;
using System.Linq;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule.Restrictions
{
    public class DayRestrictions : IScheduleRestriction, IRestriction
    {
        private List<DayOfWeek> _restrictions;

        public DayRestrictions()
        {
            this._restrictions = new List<DayOfWeek>();
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

        public bool PassesRestrictions(DateTime utcNow) =>
            this._restrictions.Any()
                ? this._restrictions.Contains(utcNow.DayOfWeek)
                : true;
    }
}