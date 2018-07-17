using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling
{
    public class SchedulerEveryTenMinuteTests
    {
        [Theory]
        [InlineData(0, 10, 20, 30)]
        [InlineData(5, 15, 25, 35)]
        [InlineData(6, 16, 27, 59)]
        [InlineData(10, 21, 32, 42)]
        public async Task ValidEveryTenMinutes(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryTenMinutes();

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);
            await RunScheduledTasksFromMinutes(scheduler, third);
            await RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.True(taskRunCount == 4);
        }

        [Theory]
        // Each case ought to run twice.
        [InlineData(0, 5, 14, 23)]
        [InlineData(15, 20, 26, 30)]

        public async Task ValidEveryTenMinutes_2RunsOnly(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryTenMinutes();

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);
            await RunScheduledTasksFromMinutes(scheduler, third);
            await RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.True(taskRunCount == 2);
        }
    }
}