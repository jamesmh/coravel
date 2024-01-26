using System;

namespace Coravel.Scheduling.Schedule.Helpers
{
    public static class DateHelpers
    {
        public static DateTime PreciseUpToSecond(this DateTime me) {
            return new DateTime(me.Year, me.Month, me.Day, me.Hour, me.Minute, me.Second, DateTimeKind.Utc);
        }
    }
}