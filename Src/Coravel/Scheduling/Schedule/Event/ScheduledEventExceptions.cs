using System;

namespace Coravel.Scheduling.Schedule.Event;
public class ScheduledEventGetInvocableException : Exception
{
    public ScheduledEventGetInvocableException(string message) : base(message)
    { }
}

public class ScheduledEventInvocableTypeException : Exception
{
    public ScheduledEventInvocableTypeException(string message) : base(message)
    { }
}
