using System;

namespace Coravel.Queuing.Interfaces
{
    public interface IHostedQueue
    {
         IHostedQueue OnError(Action<Exception> errorHandler);
    }
}