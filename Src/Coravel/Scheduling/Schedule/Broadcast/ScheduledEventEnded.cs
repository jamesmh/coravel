using System;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Event;

namespace Coravel.Scheduling.Schedule.Broadcast;

internal sealed class ScheduledEventEnded : IEvent
{
    public ScheduledEvent EndedEvent { get; }
    public DateTime EndedAtUtc { get; }

    public ScheduledEventEnded(ScheduledEvent endedEvent)
    {
        EndedEvent = endedEvent;
        EndedAtUtc = DateTime.UtcNow;
    }
}