using System;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Event;

namespace Coravel.Scheduling.Schedule.Broadcast;

public sealed class ScheduledEventStarted : IEvent
{
    public ScheduledEvent StartedEvent { get; }
    public DateTime StartedAtUtc { get; }

    public ScheduledEventStarted(ScheduledEvent startedEvent)
    {
        StartedEvent = startedEvent;
        StartedAtUtc = DateTime.UtcNow;
    }
}