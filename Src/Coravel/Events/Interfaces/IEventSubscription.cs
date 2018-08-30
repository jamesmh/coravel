namespace Coravel.Events.Interfaces
{
    public interface IEventSubscription<TEvent> where TEvent : IEvent
    {
        IEventSubscription<TEvent> Subscribe<TListener>() where TListener : IListener<TEvent>;
    }
}