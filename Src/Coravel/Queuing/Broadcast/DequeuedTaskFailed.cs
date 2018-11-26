using System;
using Coravel.Events.Interfaces;
using Coravel.Tasks;

namespace Coravel.Queuing.Broadcast
{
    public class DequeuedTaskFailed : IEvent
    {
        public ActionOrAsyncFunc DequeuedTask { get; }
        public DateTime FailedAtUtc{ get; }

        public DequeuedTaskFailed(ActionOrAsyncFunc dequeuedTask)
        {
            this.DequeuedTask =  dequeuedTask;
            this.FailedAtUtc = DateTime.UtcNow;
        }
    }
}