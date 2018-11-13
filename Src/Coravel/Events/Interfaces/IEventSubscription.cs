namespace Coravel.Events.Interfaces
{
    public interface IEventSubscription<TEvent> where TEvent : IEvent
    {
        /// <summary>
        /// Subscribe to a new listener for an event
        /// </summary>
        /// <typeparam name="TListener"></typeparam>
        /// <returns></returns>
        IEventSubscription<TEvent> Subscribe<TListener>() where TListener : IListener<TEvent>;
    }
}