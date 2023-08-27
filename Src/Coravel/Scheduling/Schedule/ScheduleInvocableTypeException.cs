using System;

namespace Coravel.Scheduling.Schedule;

public sealed class ScheduleInvocableTypeException : Exception
{
    public ScheduleInvocableTypeException(string message) : base(message)
    {

    }
}