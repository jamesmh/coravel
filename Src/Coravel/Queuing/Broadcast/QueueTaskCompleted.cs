using System;
using Coravel.Events.Interfaces;

namespace Coravel.Queuing.Broadcast
{
    public class QueueTaskCompleted : IEvent
    {
        public DateTime CompletedAtUtc { get; private set; }
        public Guid Guid { get; private set; }

        public QueueTaskCompleted(Guid id)
        {
            this.Guid = id;
            this.CompletedAtUtc = DateTime.UtcNow;
        }
    }
}