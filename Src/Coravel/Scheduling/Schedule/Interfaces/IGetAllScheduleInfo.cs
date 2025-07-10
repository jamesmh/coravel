namespace Coravel.Scheduling.Schedule.Interfaces
{
    /// <summary>
    /// Provides methods to get information about active schedules.
    /// </summary>
    public interface IGetAllScheduleInfo
    {
        /// <summary>
        /// Gets a data representation of the scheduled event.
        /// </summary>
        /// <returns>A ScheduleInfo object containing the schedule information.</returns>
        ScheduleInfo GetScheduleInfo();
    }
}