using System;
using System.Threading.Tasks;
using Coravel.Invocable;

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

        /// <summary>
        /// Schedule an Invocable job.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IScheduleInterval Schedule<T>() where T : IInvocable;
    }
}