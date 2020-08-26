using System;
using Coravel.Events.Interfaces;

namespace Coravel.Queuing.Broadcast
{
    public class QueueTaskStarted : IEvent
    {
        public DateTime StartedAtUtc { get; private set; }
        public Guid Guid { get; private set; }

        public QueueTaskStarted(Guid id)
        {
            this.Guid = id;
            this.StartedAtUtc = DateTime.UtcNow;
        }
    }
}