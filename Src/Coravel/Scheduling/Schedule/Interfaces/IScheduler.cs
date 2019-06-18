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

        /// <summary>
        /// Schedule an Invocable job.
        /// invocableType param must be assignable from and implement the IInvocable interface.
        /// </summary>
        /// <param name="invocableType"></param>
        /// <returns></returns>
        IScheduleInterval ScheduleInvocableType(Type invocableType);

        /// <summary>
        /// Begin scheduling further tasks on an isolated worker.
        /// A worker will run all scheduled tasks on it's own separate thread.
        /// </summary>
        /// <param name="workerName"></param>
        /// <returns></returns>
        IScheduler OnWorker(string workerName);
    }
}