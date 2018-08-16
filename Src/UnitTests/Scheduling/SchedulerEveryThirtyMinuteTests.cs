using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling
{
    public class SchedulerEveryThirtyMinuteTests
    {
        [Theory]
        // Should Run
        [InlineData(0, 0, 0, true)]
        [InlineData(0, 0, 30, true)]
        [InlineData(0, 0, 60, true)]
        [InlineData(1, 1, 0, true)]
        [InlineData(4, 5, 30, true)]
        [InlineData(6, 23, 60, true)]
        [InlineData(2, 22, 0, true)]
        [InlineData(3, 11, 30, true)]
        [InlineData(4, 11, 60, true)]
        // Should not run
        [InlineData(0, 0, 3, false)]
        [InlineData(0, 0, 4, false)]
        [InlineData(0, 0, 6, false)]
        [InlineData(0, 0, 7, false)]
        [InlineData(0, 0, 8, false)]
        [InlineData(3, 5, 9, false)]
        [InlineData(3, 5, 11, false)]
        [InlineData(3, 5, 12, false)]
        [InlineData(3, 5, 13, false)]
        [InlineData(3, 5, 14, false)]
        [InlineData(6, 23, 16, false)]
        [InlineData(6, 23, 17, false)]
        [InlineData(6, 23, 18, false)]
        [InlineData(6, 23, 19, false)]
        [InlineData(6, 23, 21, false)]
        [InlineData(6, 23, 22, false)]
        [InlineData(6, 23, 23, false)]
        [InlineData(1, 14, 24, false)]
        [InlineData(1, 14, 26, false)]
        [InlineData(1, 14, 27, false)]
        [InlineData(1, 14, 28, false)]
        [InlineData(1, 14, 29, false)]
        [InlineData(1, 14, 31, false)]
        [InlineData(1, 14, 49, false)]
        [InlineData(1, 14, 51, false)]
        [InlineData(1, 14, 52, false)]
        [InlineData(1, 14, 53, false)]
        [InlineData(1, 14, 54, false)]
        [InlineData(4, 7, 56, false)]
        [InlineData(4, 7, 57, false)]
        [InlineData(4, 7, 58, false)]
        [InlineData(4, 7, 59, false)]
        [InlineData(5, 6, 54, false)]
        public async Task ValidEveryThirtyMinutes(int day, int hour, int minute, bool shouldRun)
        {
            var scheduler = new Scheduler();
            bool taskRan = false;

            scheduler.Schedule(() => taskRan = true).EveryThirtyMinutes();

            await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);

            Assert.Equal(shouldRun, taskRan);
        }
    }
}