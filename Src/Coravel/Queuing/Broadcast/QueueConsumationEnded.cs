using System;
using Coravel.Events.Interfaces;

namespace Coravel.Queuing.Broadcast
{
    public class QueueConsumationEnded : IEvent
    {
        public DateTime EndedAtUtc { get; private set; }
        public QueueConsumationEnded()
        {
            this.EndedAtUtc = DateTime.UtcNow;
        }
    }
}