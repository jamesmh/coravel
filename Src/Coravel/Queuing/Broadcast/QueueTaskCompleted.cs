using System;
using Coravel.Events.Interfaces;

namespace Coravel.Queuing.Broadcast;

public sealed class QueueTaskCompleted : IEvent
{
    public DateTime CompletedAtUtc { get; }
    public Guid Guid { get; }

    public QueueTaskCompleted(Guid id)
    {
        Guid = id;
        CompletedAtUtc = DateTime.UtcNow;
    }
}