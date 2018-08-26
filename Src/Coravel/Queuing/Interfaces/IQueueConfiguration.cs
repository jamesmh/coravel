using System;
using Microsoft.Extensions.Logging;

namespace Coravel.Queuing.Interfaces
{
    /// <summary>
    /// Methods for configuring various aspects of Coravel's queue.
    /// </summary>
    public interface IQueueConfiguration
    {
        /// <summary>
        /// Global error handler.
        /// Invoked by Coravel whenever a queued task throws an Exception.
        /// </summary>
        /// <param name="errorHandler">The error handler you wish to use.</param>
        /// <returns></returns>
         IQueueConfiguration OnError(Action<Exception> errorHandler);

        /// <summary>
        /// Log the progress of queued tasks.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        IQueueConfiguration LogQueuedTaskProgress(ILogger<IQueue> logger);   
    }
}