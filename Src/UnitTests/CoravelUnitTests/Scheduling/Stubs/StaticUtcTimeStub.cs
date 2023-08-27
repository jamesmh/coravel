using System;
using Coravel.Scheduling.Schedule.Interfaces;

namespace CoravelUnitTests.Scheduling.Stubs;

public class StaticUtcTimeStub : IUtcTime
{
    private readonly DateTime _utcNow;
    public StaticUtcTimeStub(DateTime utcNow) => _utcNow = utcNow;
    public DateTime Now => _utcNow;
}