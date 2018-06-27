using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Queuing.Interfaces;
using Coravel.Tasks;
using Microsoft.Extensions.Logging;

namespace Coravel.Queuing
{
    public class Queue : IQueue, IQueueConfiguration
    {
        private ConcurrentQueue<ActionOrAsyncFunc> _tasks = new ConcurrentQueue<ActionOrAsyncFunc>();
        private Action<Exception> _errorHandler;
        private ILogger<IQueue> _logger;

        public void QueueTask(Action task)
        {
            this._tasks.Enqueue(new ActionOrAsyncFunc(task));
        }

        public void QueueAsyncTask(Func<Task> asyncTask)
        {
            this._tasks.Enqueue(new ActionOrAsyncFunc(asyncTask));
        }

        public IQueueConfiguration OnError(Action<Exception> errorHandler)
        {
            this._errorHandler = errorHandler;
            return this;
        }

        public IQueueConfiguration LogQueuedTaskProgress(ILogger<IQueue> logger)
        {
            this._logger = logger;
            return this;
        }

        public async Task ConsumeQueueAsync()
        {
            IEnumerable<ActionOrAsyncFunc> queuedTasks = this.DequeueAllTasks();

            foreach (ActionOrAsyncFunc task in queuedTasks)
            {
                try
                {
                    this._logger?.LogInformation("Queued task started...");
                    await task.Invoke();
                    this._logger?.LogInformation("Queued task finished...");
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