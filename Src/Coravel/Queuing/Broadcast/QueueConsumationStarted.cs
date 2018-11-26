using System;
using Coravel.Events.Interfaces;

namespace Coravel.Queuing.Broadcast
{
    public class QueueConsumationStarted : IEvent
    {
        public DateTime StartedAtUtc { get; }

        public QueueConsumationStarted()
        {
            this.StartedAtUtc = DateTime.UtcNow;
        }
    }
}