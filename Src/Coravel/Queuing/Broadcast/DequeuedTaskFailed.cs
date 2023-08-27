using System;
using Coravel.Events.Interfaces;
using Coravel.Tasks;

namespace Coravel.Queuing.Broadcast;

internal sealed class DequeuedTaskFailed : IEvent
{
    public ActionOrAsyncFunc DequeuedTask { get; private set; }
    public DateTime FailedAtUtc { get; private set; }
    public Guid Guid { get; private set; }

    public DequeuedTaskFailed(ActionOrAsyncFunc dequeuedTask)
    {
        DequeuedTask =  dequeuedTask;
        FailedAtUtc = DateTime.UtcNow;
        Guid = dequeuedTask.Guid;
    }
}