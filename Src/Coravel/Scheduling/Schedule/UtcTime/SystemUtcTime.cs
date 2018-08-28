using System;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule.UtcTime
{
    public class SystemUtcTime : IUtcTime
    {
        public DateTime Now => DateTime.UtcNow;
    }
}