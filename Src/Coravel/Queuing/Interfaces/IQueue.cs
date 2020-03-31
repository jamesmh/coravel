using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;
using Coravel.Invocable;

namespace Coravel.Queuing.Interfaces
{
    /// <summary>
    /// Allows queuing tasks that will be invoked automatically by Coravel for you.
    /// </summary>
    public interface IQueue
    {
        /// <summary>
        /// Queue a new synchronous task.
        /// </summary>
        /// <param name="workItem">The task to be invoke by Coravel.</param>
        void QueueTask(Action workItem);

        /// <summary>
        /// Queue a new async task.
        /// </summary>
        /// <param name="asyncItem">The async task to be invoke by Coravel.</param>
        void QueueAsyncTask(Func<Task> asyncItem);

        /// <summary>
        /// Queue an invocable that, when dequeued, will be instantiated using DI and invoked.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void QueueInvocable<T>() where T : IInvocable;

        /// <summary>
        /// Queue an invocable that, when dequeued, will be instantiated using DI and invoked.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        CancellationTokenSource QueueCancellableInvocable<T>() where T : IInvocable, ICancellableTask;

        /// <summary>
        /// Queue an event to be broadcasted.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        void QueueBroadcast<TEvent>(TEvent toBroadcast) where TEvent : IEvent;

        void QueueInvocableWithPayload<T, TParams>(TParams payload) where T : IInvocable, IInvocableWithPayload<TParams>;
    }
}