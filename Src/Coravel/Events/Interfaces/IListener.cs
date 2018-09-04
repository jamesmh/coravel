using System.Threading.Tasks;

namespace Coravel.Events.Interfaces
{
    public interface IListener<TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent broadcasted);
    }
}