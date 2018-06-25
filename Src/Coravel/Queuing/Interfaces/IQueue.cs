using System;
using System.Threading.Tasks;

namespace Coravel.Queuing.Interfaces
{
    public interface IQueue
    {
        void QueueTask(Action workItem);
        void QueueAsyncTask(Func<Task> asyncItem);
    }
}