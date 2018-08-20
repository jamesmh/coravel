using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.IntervalTests
{
    public class SchedulerHourlyAtTests
    {
        [Theory]
        // Should Run
        [InlineData(0, 0, 0, 0, true)]
        [InlineData(0, 4, 0, 0, true)]
        [InlineData(32, 4, 0, 32, true)]
        [InlineData(32, 6, 0, 32, true)]
        [InlineData(2, 4, 13, 2, true)]
        [InlineData(2, 2, 13, 2, true)]
        [InlineData(59, 4, 6, 59, true)]
        [InlineData(59, 6, 6, 59, true)]
        // Should not run
        [InlineData(59, 4, 6, 58, false)]
        [InlineData(59, 4, 7, 0, false)]
        [InlineData(59, 6, 6, 12, false)]
        [InlineData(59, 6, 7, 0, false)]
        [InlineData(59, 1, 10, 58, false)]
        [InlineData(59, 1, 7, 0, false)]
        [InlineData(59, 1, 6, 58, false)]
        [InlineData(59, 1, 6, 57, false)]
        [InlineData(0, 4, 7, 44, false)]
        [InlineData(0, 0, 0, 1, false)]
        [InlineData(1, 0, 0, 0, false)]
        [InlineData(0, 2, 23, 58, false)]
        [InlineData(0, 2, 23, 59, false)]

        public async Task HourlyAtTests(int atMinute, int day, int hour, int minute, bool shouldRun)
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub());
            bool taskRan = false;

            scheduler.Schedule(() => taskRan = true).HourlyAt(atMinute);

            await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);

            Assert.Equal(shouldRun, taskRan);
        }
    }
}