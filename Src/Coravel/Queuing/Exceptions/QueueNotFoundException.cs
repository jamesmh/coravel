using System;

namespace Coravel.Queuing.Exceptions
{
    public class QueueNotFoundException : Exception
    {
        public QueueNotFoundException(string name): base($"Queue with name '{name}' doesn't exist!")
        {
            
        }
    }
}