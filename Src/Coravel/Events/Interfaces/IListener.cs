using System.Threading.Tasks;

namespace Coravel.Events.Interfaces
{
    public interface IListener<TEvent> : IListener
        where TEvent : IEvent
    {
        Task HandleAsync(TEvent broadcasted);

        Task IListener.HandleAsync(IEvent broadcasted)
             => HandleAsync((TEvent)broadcasted);
    }

    public interface IListener
    {
        internal Task HandleAsync(IEvent broadcasted);
    }
}