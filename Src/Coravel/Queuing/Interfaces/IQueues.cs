using System.Collections;

namespace Coravel.Queuing.Interfaces
{
    public interface IQueues
    {
        IQueue Get(string queueName);
    }
}