using System;
using Coravel.Events.Interfaces;
using Coravel.Tasks;

namespace Coravel.Queuing.Broadcast
{
    public class DequeuedTaskFailed : IEvent
    {
        public ActionOrAsyncFunc DequeuedTask { get; private set; }
        public DateTime FailedAtUtc { get; private set; }
        public Guid Guid { get; private set; }

        public DequeuedTaskFailed(ActionOrAsyncFunc dequeuedTask)
        {
            this.DequeuedTask =  dequeuedTask;
            this.FailedAtUtc = DateTime.UtcNow;
            this.Guid = dequeuedTask.Guid;
        }
    }
}