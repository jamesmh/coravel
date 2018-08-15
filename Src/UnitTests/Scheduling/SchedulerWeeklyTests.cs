using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling
{
    public class SchedulerWeeklyTests
    {
        [Theory]
        // Note: arrays are [day, hours, minutes]
        [InlineData(new int[] { 0, 0, 0 }, new int[] { 7, 0, 0 })]
        public async Task ValidWeekly(int[] first, int[] second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).Weekly();

            await RunScheduledTasksFromDayHourMinutes(scheduler, first[0], first[1], first[2]);
            await RunScheduledTasksFromDayHourMinutes(scheduler, second[0], second[1], second[2]);

            Assert.True(taskRunCount == 2);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(1, 5)]
        [InlineData(1, 6)]
        [InlineData(6, 1)]
        [InlineData(1, 0)]
        public async Task Weekly_ShouldRunOnce(int first, int second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).Weekly();

            await RunScheduledTasksFromDayHourMinutes(scheduler, first, 0, 0);
            await RunScheduledTasksFromDayHourMinutes(scheduler, second, 0, 0);

            Assert.True(taskRunCount == 1);
        }
    }
}