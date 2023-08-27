using System;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule.UtcTime;

internal sealed class SystemUtcTime : IUtcTime
{
    public DateTime Now => DateTime.UtcNow;
}