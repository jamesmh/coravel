using System;
using Coravel.Scheduling.Schedule.Interfaces;

namespace UnitTests.Scheduling.Stubs
{
    public class StaticUtcTimeStub : IUtcTime
    {
        private readonly DateTime _utcNow;
        public StaticUtcTimeStub(DateTime utcNow)
        {
            this._utcNow = utcNow;
        }
        public DateTime Now => this._utcNow;
    }
}