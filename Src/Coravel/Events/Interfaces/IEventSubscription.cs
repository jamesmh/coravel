namespace Coravel.Events.Interfaces;

/// <summary>
/// Defines a contract for an event subscription that allows subscribing listeners.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public interface IEventSubscription<TEvent> where TEvent : IEvent
{
    /// <summary>
    /// Subscribes a listener to the event subscription.
    /// </summary>
    /// <typeparam name="TListener">The type of the listener.</typeparam>
    /// <returns>The event subscription.</returns>
    IEventSubscription<TEvent> Subscribe<TListener>() where TListener : IListener<TEvent>;
}
