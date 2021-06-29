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
        /// Schedule an Invocable job with a list of parameters.
        /// Parameters are injected into the constructor of the Invocable while remaining dependencies are resolved from DI.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">Parameters to inject.</param>
        /// <returns></returns>
        IScheduleInterval ScheduleWithParams<T>(params object[] parameters) where T : IInvocable;

        /// <summary>
        /// Schedule an Invocable job with a list of parameters for the specified <paramref name="invocableType"/>.
        /// Parameters are injected into the constructor of the Invocable while remaining dependencies are resolved from DI.
        /// </summary>
        /// <param name="invocableType">Type of the invocable.</param>
        /// <param name="parameters">Parameters to inject.</param>
        /// <exception cref="ArgumentException">If the <paramref name="invocableType"/> does not implements or inherits from <see cref="IInvocable"/>.</exception>
        IScheduleInterval ScheduleWithParams(Type invocableType, params object[] parameters);

        /// <summary>
        /// Schedule an Invocable job.
        /// InvocableType param must be assignable from and implement the IInvocable interface.
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