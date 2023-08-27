using System;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Event;

namespace Coravel.Scheduling.Schedule.Broadcast;

internal sealed class ScheduledEventFailed : IEvent
{
    public ScheduledEvent FailedEvent { get; }
    public DateTime FailedAtUtc { get; private set; }
    public Exception Exception { get; }

    public ScheduledEventFailed(ScheduledEvent failedEvent, Exception ex)
    {
        FailedEvent = failedEvent;
        FailedAtUtc = DateTime.UtcNow;
        Exception = ex;
    }
}