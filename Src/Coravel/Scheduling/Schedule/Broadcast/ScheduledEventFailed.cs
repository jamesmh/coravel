using System;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Event;

namespace Coravel.Scheduling.Schedule.Broadcast
{
    public class ScheduledEventFailed : IEvent
    {
        public ScheduledEvent FailedEvent { get; private set; }
        public DateTime FailedAtUtc { get; private set; }
        public Exception Exception { get; private set; }

        public ScheduledEventFailed(ScheduledEvent failedEvent, Exception ex)
        {
            this.FailedEvent = failedEvent;
            this.FailedAtUtc = DateTime.UtcNow;
            this.Exception = ex;
        }
    }
}