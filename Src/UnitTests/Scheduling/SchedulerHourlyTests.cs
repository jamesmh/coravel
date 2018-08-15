using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling
{
    public class SchedulerHourlyTests
    {
        [Theory]
        [InlineData(0)]
                [InlineData(60)]
                        [InlineData(120)]
        public async Task ValidHourly(int runAt)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).Hourly();

            await RunScheduledTasksFromMinutes(scheduler, runAt);

            Assert.True(taskRunCount == 1);
        }

        [Theory]
        [InlineData(0, 25, 30, 59)]
        [InlineData(5, 30, 60, 64)]

        public async Task ValidHourly_OneRun(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).Hourly();

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);
            await RunScheduledTasksFromMinutes(scheduler, third);
            await RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.True(taskRunCount == 1);
        }
    }
}