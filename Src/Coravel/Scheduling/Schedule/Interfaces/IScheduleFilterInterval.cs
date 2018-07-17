using System;

namespace Coravel.Scheduling.Schedule.Interfaces
{
    /// <summary>
    /// Defines methods available to you for specifying
    /// the frequency of how often your
    /// scheduled tasks will run.
    /// </summary>
    public interface IScheduleFilterInterval : IScheduleInterval
    {
        IScheduleInterval When(Func<bool> func);
    }
}