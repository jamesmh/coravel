namespace Coravel.Events.Interfaces;

public interface IEventRegistration
{
    /// <summary>
    /// Register a new event for Coravel's Dispatcher.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    IEventSubscription<TEvent> Register<TEvent>() where TEvent : IEvent;
}