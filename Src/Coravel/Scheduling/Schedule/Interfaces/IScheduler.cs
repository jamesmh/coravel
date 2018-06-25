using System;
using System.Threading.Tasks;

namespace Coravel.Scheduling.Schedule.Interfaces
{
    public interface IScheduler
    {
        IScheduleInterval Schedule(Action actionToSchedule);
        IScheduleInterval ScheduleAsync(Func<Task> asyncTaskToSchedule);
    }
}