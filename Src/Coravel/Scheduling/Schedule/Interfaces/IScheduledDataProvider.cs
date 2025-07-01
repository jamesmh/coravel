namespace Coravel.Scheduling.Schedule.Interfaces
{
    /// <summary>
    /// Provides a method to get scheduled data representation.
    /// </summary>
    public interface IScheduledDataProvider
    {
        /// <summary>
        /// Gets a data representation of the scheduled event.
        /// </summary>
        /// <returns>A ScheduledData object containing the schedule information.</returns>
        ScheduledData GetScheduledData();
    }
}