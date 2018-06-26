using System;
using System.Threading.Tasks;

namespace Coravel.Scheduling.Schedule.Interfaces
{
    /// <summary>
    /// Provides methods for scheduling tasks using Coravel.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Schedule a task.
        /// </summary>
        /// <param name="actionToSchedule">Task to schedule.</param>
        /// <returns></returns>
        IScheduleInterval Schedule(Action actionToSchedule);

        /// <summary>
        /// Schedule an asynchronous task.
        /// </summary>
        /// <param name="asyncTaskToSchedule">Async task to schedule.</param>
        /// <returns></returns>
        IScheduleInterval ScheduleAsync(Func<Task> asyncTaskToSchedule);
    }
}