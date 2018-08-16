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
        // Should run
        [InlineData(0, 0, 0, true)]
        [InlineData(1, 4, 0, true)]
        [InlineData(2, 8, 0, true)]
        [InlineData(3, 13, 0, true)]
        [InlineData(4, 21, 0, true)]
        [InlineData(5, 23, 0, true)]
        [InlineData(6, 1, 0, true)]
        // Should not run
        [InlineData(0, 0, 1, false)]
        [InlineData(1, 4, 12, false)]
        [InlineData(2, 8, 30, false)]
        [InlineData(3, 13, 33, false)]
        [InlineData(4, 21, 59, false)]
        [InlineData(5, 23, 58, false)]
        [InlineData(6, 1, 44, false)]
        [InlineData(0, 0, 55, false)]
        [InlineData(1, 4, 31, false)]
        [InlineData(2, 8, 12, false)]
        [InlineData(3, 13, 1, false)]
        [InlineData(4, 21, 2, false)]
        [InlineData(5, 23, 3, false)]
        [InlineData(6, 1, 4, false)]
        [InlineData(0, 0, 15, false)]
        [InlineData(1, 4, 20, false)]
        [InlineData(2, 8, 25, false)]
        [InlineData(3, 13, 30, false)]
        [InlineData(4, 21, 45, false)]
        [InlineData(5, 23, 55, false)]
        [InlineData(6, 1, 1, false)]

        public async Task ValidHourly(int day, int hour, int minute, bool shouldRun)
        {
            var scheduler = new Scheduler();
            bool taskRan = false;

            scheduler.Schedule(() => taskRan = true).Hourly();

            await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);

            Assert.Equal(shouldRun, taskRan);
        }
    }
}