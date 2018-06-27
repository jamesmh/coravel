using System;
using Microsoft.Extensions.Logging;

namespace Coravel.Scheduling.Schedule.Interfaces
{
    /// <summary>
    /// Methods for configuring various aspects of Coravel's scheduler.
    /// </summary>
    public interface ISchedulerConfiguration
    {
        /// <summary>
        /// Global error handler invoked whenever a scheduled task throws an exception.
        /// </summary>
        /// <param name="onError">Error handler to invoke on error.</param>
        /// <returns></returns>
        ISchedulerConfiguration OnError(Action<Exception> onError);

        ISchedulerConfiguration LogScheduledTaskProgress(ILogger<IScheduler> logger);        
    }
}