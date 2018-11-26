using System.Threading.Tasks;

namespace Coravel.Events.Interfaces
{
    public interface IListener<TEvent> where TEvent : IEvent
    {
        /// <summary>
        /// Handles an event
        /// </summary>
        /// <param name="broadcasted"></param>
        /// <returns></returns>
        Task HandleAsync(TEvent broadcasted);
    }
}