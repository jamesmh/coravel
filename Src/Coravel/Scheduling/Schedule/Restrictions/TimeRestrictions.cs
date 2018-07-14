using System;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule.Restrictions
{
    public class TimeRestrictions
    {
        private int? _hourRestriction = null;
        private int? _minuteRestriction = null;

        public void OccursAtMinute(int minute) {
            if(ValidMinutes(minute))
                this._minuteRestriction = minute;
            else
                throw new Exception("When restricting minutes please specify a minutes value between 0 and 59");          
        }

        public void OccursAtHour(int hour) {
            if(ValidHours(hour))
                this._hourRestriction = hour;
            else
                throw new Exception("When restricting hours please specify a hours value between 0 and 23");          
        }

        public void OccursAt(int hours, int minutes) {
            this.OccursAtHour(hours);
            this.OccursAtMinute(minutes);
        }

        public bool PassesRestrictions(DateTime utcNow) {
            int validHour = this._hourRestriction ?? utcNow.Hour;
            int validMinute = this._minuteRestriction ?? utcNow.Minute;

            return utcNow.Hour == validHour 
                && utcNow.Minute == validMinute;
        }

        private bool ValidMinutes(int minutes) => minutes >= 0 && minutes <= 59;
        private bool ValidHours(int hours) => hours >= 0 && hours <= 23;
    }
}