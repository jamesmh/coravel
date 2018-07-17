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
        [InlineData(new int[] { 0, 0, 0 }, new int[] { 8, 1, 0 })]
        [InlineData(new int[] { 3, 0, 32 }, new int[] { 10, 0, 33 })]
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
        // Note: arrays are [day, hours, minutes]
        [InlineData(new int[] { 0, 0, 0 }, new int[] { 6, 0, 0 })]
        [InlineData(new int[] { 0, 5, 0 }, new int[] { 7, 4, 59 })]
        [InlineData(new int[] { 0, 5, 0 }, new int[] { 2, 5, 59 })]
        public async Task Weekly_ShouldRunOnce(int[] first, int[] second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).Weekly();

            await RunScheduledTasksFromDayHourMinutes(scheduler, first[0], first[1], first[2]);
            await RunScheduledTasksFromDayHourMinutes(scheduler, second[0], second[1], second[2]);

            Assert.True(taskRunCount == 1);
        }
    }
}