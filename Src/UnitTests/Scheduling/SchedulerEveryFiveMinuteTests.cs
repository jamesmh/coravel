using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling
{
    public class SchedulerEveryFiveMinuteTests
    {
        [Theory]
        [InlineData(0, 5, 10, 15)]
        [InlineData(20, 25, 30, 35)]
        [InlineData(4, 10, 21, 50)]
        [InlineData(30, 37, 50, 59)]
        public async Task ValidEveryFiveMinutes(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryFiveMinutes();

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);
            await RunScheduledTasksFromMinutes(scheduler, third);
            await RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.True(taskRunCount == 4);
        }

        [Theory]
        // Each case ought to run twice.
        [InlineData(0, 3, 4, 5)]
        [InlineData(15, 16, 17, 30)]


        public async Task ValidEveryFiveMinutes_2RunsOnly(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryFiveMinutes();

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);
            await RunScheduledTasksFromMinutes(scheduler, third);
            await RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.True(taskRunCount == 2);
        }
    }
}