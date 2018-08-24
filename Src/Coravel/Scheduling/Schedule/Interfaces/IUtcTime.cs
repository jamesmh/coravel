using System;

namespace Coravel.Scheduling.Schedule.Interfaces
{
    public interface IUtcTime
    {
        DateTime Now { get; }
    }
}