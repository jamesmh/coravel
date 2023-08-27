using System;

namespace Coravel.Scheduling.Schedule.Helpers;

/// <summary>
/// Provides helper methods for working with dates.
/// </summary>
public static class DateHelpers
{
    /// <summary>
    /// Returns a new DateTime object that is precise up to the minute.
    /// </summary>
    /// <param name="me">The original DateTime object.</param>
    /// <returns>A new DateTime object with the same year, month, day, hour and minute as the original, but with zero seconds and milliseconds.</returns>
    public static DateTime PreciseUpToMinute(this DateTime me)
    {
        return new DateTime(me.Year, me.Month, me.Day, me.Hour, me.Minute, 0, DateTimeKind.Utc);
    }
}
