using System;

namespace Coravel.Scheduling.Schedule.Interfaces
{
    public interface ISchedulerConfiguration
    {
        ISchedulerConfiguration OnError(Action<Exception> onError);
    }
}