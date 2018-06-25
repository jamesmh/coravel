using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Queuing.Interfaces;
using Coravel.Tasks;

namespace Coravel.Queuing
{
    public class Queue : IQueue, IHostedQueue
    {
        private ConcurrentQueue<ActionOrAsyncFunc> _tasks = new ConcurrentQueue<ActionOrAsyncFunc>();
        private Action<Exception> _errorHandler;

        public void QueueTask(Action task)
        {
            this._tasks.Enqueue(new ActionOrAsyncFunc(task));
        }

        public void QueueTaskAsync(Func<Task> asyncTask)
        {
            this._tasks.Enqueue(new ActionOrAsyncFunc(asyncTask));
        }

        public IHostedQueue OnError(Action<Exception> errorHandler)
        {
            this._errorHandler = errorHandler;
            return this;
        }

        public async Task ConsumeQueueAsync()
        {
            IEnumerable<ActionOrAsyncFunc> queuedTasks = this.DequeueAllTasks();

            foreach (ActionOrAsyncFunc task in queuedTasks)
            {
                try
                {
                    await task.Invoke();
                }
                catch (Exception e)
                {
                    if (this._errorHandler != null)
                    {
                        this._errorHandler(e);
                    }
                }
            }
        }

        private IEnumerable<ActionOrAsyncFunc> DequeueAllTasks()
        {
            while (this._tasks.TryPeek(out var dummy))
            {
                this._tasks.TryDequeue(out var queuedTask);
                yield return queuedTask;
            }
        }
    }
}