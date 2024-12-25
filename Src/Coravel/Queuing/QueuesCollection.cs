using System.Collections;
using System.Collections.Generic;
using Coravel.Queuing.Interfaces;
using System.Linq;
using Coravel.Queuing.Exceptions;

namespace Coravel.Queuing
{
    public class QueuesCollection : IQueues
    {
        private readonly IEnumerable<IQueue> _queues;

        public QueuesCollection(IEnumerable<IQueue> queues)
        {
            _queues = queues;
        }
        public IQueue Get(string queueName)
        {
            if (_queues.FirstOrDefault(x => x.QueueName == queueName) is IQueue queue)
            {
                return queue;
            }
            throw new QueueNotFoundException(queueName);
        }
    }
}