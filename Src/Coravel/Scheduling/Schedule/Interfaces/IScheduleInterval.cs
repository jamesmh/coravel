namespace Coravel.Scheduling.Schedule.Interfaces
{
    /// <summary>
    /// Defines methods available to you for specifying
    /// the frequency of how often your
    /// scheduled tasks will run.
    /// </summary>
    public interface IScheduleInterval
    {
        /// <summary>
        /// Scheduled task runs every minute.
        /// </summary>
        /// <returns></returns>
        IScheduleRestriction EveryMinute();

        /// <summary>
        /// Scheduled task runs every five minutes.
        /// </summary>
        /// <returns></returns>
        IScheduleRestriction EveryFiveMinutes();

        /// <summary>
        /// Scheduled task runs every ten minutes.
        /// </summary>
        /// <returns></returns>
        IScheduleRestriction EveryTenMinutes();

        /// <summary>
        /// Scheduled task runs every fifteen minutes.
        /// </summary>
        /// <returns></returns>
        IScheduleRestriction EveryFifteenMinutes();

        /// <summary>
        /// Scheduled task runs every thirty minutes.
        /// </summary>
        /// <returns></returns>
        IScheduleRestriction EveryThirtyMinutes();

        /// <summary>
        /// Scheduled task runs once an hour.
        /// </summary>
        /// <returns></returns>
        IScheduleRestriction Hourly();

        /// <summary>
        /// Scheduled task runs once an hour, but only at the time specified.
        /// </summary>
        /// <example>
        /// HourlyAt(14); // Will run once an hour at xx:14.
        /// </example>
        /// <param name="minute">Minute each hour that task will run.</param>
        /// <returns></returns>
        IScheduleRestriction HourlyAt(int minute);

        /// <summary>
        /// Scheduled task runs once a day.
        /// </summary>
        /// <returns></returns>
        IScheduleRestriction Daily();

        /// <summary>
        /// Scheduled task runs once a day at the hour specified.
        /// </summary>
        /// <example>
        /// DailyAtHour(13); // Run task daily at 1 pm utc.
        /// </example>
        /// <param name="hour">Task only runs at this hour.</param>
        /// <returns></returns>
        IScheduleRestriction DailyAtHour(int hour);

        /// <summary>
        /// Scheduled task runs once a day at the time specified.
        /// </summary>
        /// <example>
        /// DailyAt(13, 01); // Run task daily at 1:01 pm utc.
        /// </example>
        /// <param name="hour">Task only runs at this hour.</param>
        /// <param name="minute">Task only runs at time with this minute.</param>
        /// <returns></returns>
        IScheduleRestriction DailyAt(int hour, int minute);

        /// <summary>
        /// Scheduled task runs once a week.
        /// </summary>
        /// <returns></returns>
        IScheduleRestriction Weekly();
    }
}