using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling
{
    public class SchedulerEveryFifteenMinuteTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(15)]
        [InlineData(30)]
        [InlineData(45)]
        public async Task ValidEveryFifteenMinutes(int runAt)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryFifteenMinutes();

            await RunScheduledTasksFromMinutes(scheduler, runAt);

            Assert.True(taskRunCount == 1);
        }

        [Theory]
        [InlineData(0, 5, 30, 59)]
        [InlineData(15, 20, 29, 30)]
        [InlineData(30, 44, 45, 59)]

        public async Task ValidEveryFifteenMinutes_2RunsOnly(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryFifteenMinutes();

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);
            await RunScheduledTasksFromMinutes(scheduler, third);
            await RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.True(taskRunCount == 2);
        }
    }
}