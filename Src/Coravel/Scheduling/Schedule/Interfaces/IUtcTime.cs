using System;

namespace Coravel.Scheduling.Schedule.Interfaces
{
    /// <summary>
    /// Abstractions for grabbing the current UTC time.
    /// </summary>
    public interface IUtcTime
    {
        /// <summary>
        /// Get UTC time right now.
        /// </summary>
        /// <value></value>
        DateTime Now { get; }
    }
}