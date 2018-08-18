using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling
{
    public class SchedulerDailyTests
    {
        [Theory]
        // Note: arrays are [day, hours, minutes]
        [InlineData(new int[] { 0, 0, 0 }, new int[] { 1, 0, 0 })]
        [InlineData(new int[] { 0, 0, 0 }, new int[] { 1, 1, 0 })]
        [InlineData(new int[] { 0, 6, 0 }, new int[] { 1, 7, 0 })]
        [InlineData(new int[] { 0, 1, 0 }, new int[] { 1, 1, 0 })]
        [InlineData(new int[] { 0, 0, 5 }, new int[] { 1, 0, 5 })]
        [InlineData(new int[] { 0, 0, 5 }, new int[] { 1, 0, 55 })]
        public async Task ValidDaily(int[] first, int[] second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).Daily();

            await RunScheduledTasksFromDayHourMinutes(scheduler, first[0], first[1], first[2]);
            await RunScheduledTasksFromDayHourMinutes(scheduler, second[0], second[1], second[2]);

            Assert.True(taskRunCount == 2);
        }

        [Theory]
        // Note: arrays are [day, hours, minutes]
        [InlineData(new int[] { 0, 0, 0 }, new int[] { 0, 23, 59 })]
        [InlineData(new int[] { 0, 5, 0 }, new int[] { 1, 4, 0 })]
        [InlineData(new int[] { 0, 5, 0 }, new int[] { 1, 4, 59 })]
        public async Task Daily_ShouldRunOnce(int[] first, int[] second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).Daily();

            await RunScheduledTasksFromDayHourMinutes(scheduler, first[0], first[1], first[2]);
            await RunScheduledTasksFromDayHourMinutes(scheduler, second[0], second[1], second[2]);

            Assert.True(taskRunCount == 1);
        }
    }
}