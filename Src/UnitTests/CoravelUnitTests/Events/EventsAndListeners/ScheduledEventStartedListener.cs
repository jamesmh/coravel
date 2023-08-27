using System.Threading.Tasks;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Broadcast;

namespace CoravelUnitTests.Events.EventsAndListeners;

public class ScheduledEventStartedListener : IListener<ScheduledEventStarted>
{
    public static bool Ran = false;

    public Task HandleAsync(ScheduledEventStarted broadcasted)
    {
        Ran = true;
        return Task.CompletedTask;
    }
}