using System.Threading.Tasks;

namespace Coravel.Events.Interfaces;

/// <summary>
/// Defines a contract for a listener that can handle an event asynchronously.
/// </summary>
/// <typeparam name="TEvent">The type of the event.</typeparam>
public interface IListener<in TEvent> where TEvent : IEvent
{
    /// <summary>
    /// Handles the event asynchronously.
    /// </summary>
    /// <param name="broadcasted">The event that was broadcasted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleAsync(TEvent broadcasted);
}
