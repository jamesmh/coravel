using System;

namespace Coravel.Scheduling.Schedule.Helpers
{
    public static class DateHelpers
    {
        public static DateTime PreciseUpToMinute(this DateTime me) {
            return new DateTime(me.Year, me.Month, me.Day, me.Hour, me.Minute, 0);
        }
    }
}