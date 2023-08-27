using System;
using Coravel.Events.Interfaces;

namespace Coravel.Queuing.Broadcast;

public sealed class QueueTaskStarted : IEvent
{
    public DateTime StartedAtUtc { get; }
    public Guid Guid { get; }

    public QueueTaskStarted(Guid id)
    {
        Guid = id;
        StartedAtUtc = DateTime.UtcNow;
    }
}