using System;

namespace Coravel.Scheduling.Schedule.Interfaces
{
    public interface IHostedScheduler
    {
        IHostedScheduler OnError(Action<Exception> onError);
    }
}