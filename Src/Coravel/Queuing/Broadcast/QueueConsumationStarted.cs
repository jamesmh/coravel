using System;
using Coravel.Events.Interfaces;

namespace Coravel.Queuing.Broadcast;

public sealed class QueueConsumationStarted : IEvent
{
    public DateTime StartedAtUtc { get; private set; }
    public QueueConsumationStarted() => StartedAtUtc = DateTime.UtcNow;
}