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
        IScheduledEventConfiguration EveryMinute();

        /// <summary>
        /// Scheduled task runs every five minutes.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration EveryFiveMinutes();

        /// <summary>
        /// Scheduled task runs every ten minutes.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration EveryTenMinutes();

        /// <summary>
        /// Scheduled task runs every fifteen minutes.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration EveryFifteenMinutes();

        /// <summary>
        /// Scheduled task runs every thirty minutes.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration EveryThirtyMinutes();

        /// <summary>
        /// Scheduled task runs once an hour.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration Hourly();

        /// <summary>
        /// Scheduled task runs once an hour, but only at the time specified.
        /// </summary>
        /// <example>
        /// HourlyAt(14); // Will run once an hour at xx:14.
        /// </example>
        /// <param name="minute">Minute each hour that task will run.</param>
        /// <returns></returns>
        IScheduledEventConfiguration HourlyAt(int minute);

        /// <summary>
        /// Scheduled task runs once a day.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration Daily();

        /// <summary>
        /// Scheduled task runs once a day at a random time.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration DailyAtRandomTime();        
        
        /// <summary>
        /// Scheduled task runs once a day at the hour specified.
        /// </summary>
        /// <example>
        /// DailyAtHour(13); // Run task daily at 1 pm utc.
        /// </example>
        /// <param name="hour">Task only runs at this hour.</param>
        /// <returns></returns>
        IScheduledEventConfiguration DailyAtHour(int hour);

        /// <summary>
        /// Scheduled task runs once a day at the time specified.
        /// </summary>
        /// <example>
        /// DailyAt(13, 01); // Run task daily at 1:01 pm utc.
        /// </example>
        /// <param name="hour">Task only runs at this hour.</param>
        /// <param name="minute">Task only runs at time with this minute.</param>
        /// <returns></returns>
        IScheduledEventConfiguration DailyAt(int hour, int minute);

        /// <summary>
        /// Scheduled task runs once a week.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration Weekly();

        /// <summary>
        /// Schedule an event from a basic cron expression.
        /// Supported values for expression parts are:
        /// - "*"
        /// - "5"
        /// - "5,6,7"
        /// - "5-10"
        /// - "*/10"
        /// 
        /// For example "* * * * 0" would schedule an event to run every minute on Sundays.
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <returns></returns>
        IScheduledEventConfiguration Cron(string cronExpression);

        /// <summary>
        /// Scheduled task runs once a second.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration EverySecond();

        /// <summary>
        /// Scheduled task runs once every five seconds.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration EveryFiveSeconds();

        /// <summary>
        /// Scheduled task runs once every ten seconds.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration EveryTenSeconds();

        /// <summary>
        /// Scheduled task runs once every fifteen seconds.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration EveryFifteenSeconds();

        /// <summary>
        /// Scheduled task runs once every thirty seconds.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration EveryThirtySeconds();

        /// <summary>
        /// Scheduled task runs once every N seconds.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration EverySeconds(int seconds);
    }
}