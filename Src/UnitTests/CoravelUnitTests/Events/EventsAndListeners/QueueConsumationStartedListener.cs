using System.Threading.Tasks;
using Coravel.Events.Interfaces;
using Coravel.Queuing.Broadcast;

namespace CoravelUnitTests.Events.EventsAndListeners;

public class QueueConsumationStartedListener : IListener<QueueConsumationStarted>
{
    public static bool Ran = false;

    public Task HandleAsync(QueueConsumationStarted broadcasted)
    {
        Ran = true;
        return Task.CompletedTask;
    }
}