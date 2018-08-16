namespace Coravel.Scheduling.Schedule.Interfaces
{
    /// <summary>
    /// Provides methods to restrict scheduled tasks to only run on certain day(s).
    /// </summary>
    public interface IScheduleOptions
    {
        /// <summary>
        /// Restrict task to run on Mondays.
        /// </summary>
        /// <returns></returns>
        IScheduleOptions Monday();

        /// <summary>
        /// Restrict task to run on Tuesdays.
        /// </summary>
        /// <returns></returns>
        IScheduleOptions Tuesday();

        /// <summary>
        /// Restrict task to run on Wednesdays.
        /// </summary>
        /// <returns></returns>
        IScheduleOptions Wednesday();

        /// <summary>
        /// Restrict task to run on Thursdays.
        /// </summary>
        /// <returns></returns>
        IScheduleOptions Thursday();

        /// <summary>
        /// Restrict task to run on Fridays.
        /// </summary>
        /// <returns></returns>
        IScheduleOptions Friday();

        /// <summary>
        /// Restrict task to run on Saturdays.
        /// </summary>
        /// <returns></returns>
        IScheduleOptions Saturday();

        /// <summary>
        /// Restrict task to run on Sundays.
        /// </summary>
        /// <returns></returns>
        IScheduleOptions Sunday();

        /// <summary>
        /// Restrict task to run on weekdays (Monday - Friday).
        /// </summary>
        /// <returns></returns>
        IScheduleOptions Weekday();

        /// <summary>
        /// Restrict task to run on weekends (Saturday and Sunday).
        /// </summary>
        /// <returns></returns>
        IScheduleOptions Weekend();
    }
}