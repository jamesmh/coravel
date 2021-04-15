using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.IntervalTests
{
    public class SchedulerDailyAtRandomTimeTests
    {
        [Theory]
        // Should run
        [InlineData(1, 0, 1, false)]
        [InlineData(2, 0, 1, false)]
        [InlineData(3, 0, 1, false)]
        [InlineData(3, 1, 1, false)]
        [InlineData(3, 2, 1, false)]
        [InlineData(1, 4, 1, false)]
        [InlineData(2, 5, 1, false)]
        [InlineData(10, 6, 1, false)]
        [InlineData(23, 7, 1, false)]
        [InlineData(33, 8, 1, false)]
        [InlineData(30, 12, 1, false)]
        [InlineData(45, 13, 1, false)]
        [InlineData(50, 15, 1, false)]
        [InlineData(59, 23, 1, false)]
        public async Task ValidDaily(int day, int hour, int minute, bool shouldRun)
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            bool taskRan = false;

            scheduler.Schedule(() => taskRan = true).DailyAtRandomTime();

            await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);

            Assert.Equal(shouldRun, taskRan);
        }
    }
}