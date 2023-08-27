using System;
using Coravel.Events.Interfaces;

namespace Coravel.Queuing.Broadcast;

internal sealed class QueueConsumationEnded : IEvent
{
    public DateTime EndedAtUtc { get; private set; }
    public QueueConsumationEnded() => EndedAtUtc = DateTime.UtcNow;
}