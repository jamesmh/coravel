using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling
{
    public class SchedulerEveryMinuteTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(15)]
        [InlineData(30)]
        [InlineData(59)]
        public async Task ValidEveryMinute(int minutes)
        {
            var scheduler = new Scheduler();
            bool wasRun = false;

            scheduler.Schedule(() => wasRun = true).EveryMinute();
            await scheduler.RunAtAsync(DateTime.Today.Add(TimeSpan.FromMinutes(minutes)));

            Assert.True(wasRun);
        }
    }
}