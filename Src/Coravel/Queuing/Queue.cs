using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Queuing.Interfaces;

namespace Coravel.Queuing
{
    public class Queue : IQueue
    {
        private ConcurrentQueue<Action> _tasks = new ConcurrentQueue<Action>();

        public void QueueTask(Action task)
        {
            this._tasks.Enqueue(task);
        }

        public IEnumerable<Action> DequeueAllTasks(){
            while(this._tasks.TryPeek(out var dummy)) {
                this._tasks.TryDequeue(out var queuedTask);
                yield return queuedTask;
            }
        }
    }
}