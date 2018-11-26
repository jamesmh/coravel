using System;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.Schedule.Event;

namespace Coravel.Scheduling.Schedule.Broadcast
{
    public class ScheduledEventEnded : IEvent
    {
        public ScheduledEvent EndedEvent { get; private set; }
        public DateTime EndedAtUtc { get; private set; }

        public ScheduledEventEnded(ScheduledEvent endedEvent)
        {
            this.EndedEvent = endedEvent;
            this.EndedAtUtc = DateTime.UtcNow;
        }
    }
}