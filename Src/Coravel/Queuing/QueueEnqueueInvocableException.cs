using System;

namespace Coravel.Queuing;

public sealed class QueueEnqueueInvocableException : Exception
{
    public QueueEnqueueInvocableException(string message) : base(message)
    {

    }
}