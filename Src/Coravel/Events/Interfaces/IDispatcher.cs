using System.Threading.Tasks;

namespace Coravel.Events.Interfaces
{
    /// <summary>
    /// Dispatcher will dispatch Coravel events and broadcast them to the appropriate listeners.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Dispatches and broadcasts the event to all subscribed listeners.
        /// </summary>
        Task Broadcast(IEvent toBroadcast);
    }
}