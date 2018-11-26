using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.IntervalTests
{
    public class SchedulerEveryFiveMinuteTests
    {
        [Theory]
        // Should Run
        [InlineData(0, 0, 0, true)]
        [InlineData(0, 0, 5, true)]
        [InlineData(0, 0, 10, true)]
        [InlineData(0, 0, 15, true)]
        [InlineData(0, 0, 20, true)]
        [InlineData(0, 0, 25, true)]
        [InlineData(0, 0, 30, true)]
        [InlineData(0, 0, 35, true)]
        [InlineData(0, 0, 45, true)]
        [InlineData(0, 0, 50, true)]
        [InlineData(0, 0, 55, true)]
        [InlineData(0, 0, 60, true)]
        [InlineData(0, 7, 0, true)]
        [InlineData(3, 7, 0, true)]
        [InlineData(3, 7, 15, true)]
        [InlineData(5, 23, 5, true)]
        [InlineData(5, 23, 15, true)]
        [InlineData(5, 23, 30, true)]
        [InlineData(5, 23, 45, true)]
        // Should not run
        [InlineData(0, 0, 1, false)]
        [InlineData(0, 0, 2, false)]
        [InlineData(0, 0, 3, false)]
        [InlineData(0, 0, 4, false)]
        [InlineData(0, 0, 6, false)]
        [InlineData(0, 0, 7, false)]
        [InlineData(0, 0, 8, false)]
        [InlineData(0, 0, 9, false)]
        [InlineData(0, 0, 11, false)]
        [InlineData(0, 0, 12, false)]
        [InlineData(0, 0, 13, false)]
        [InlineData(0, 0, 14, false)]
        [InlineData(0, 0, 16, false)]
        [InlineData(0, 0, 17, false)]
        [InlineData(0, 0, 18, false)]
        [InlineData(0, 0, 19, false)]
        [InlineData(0, 0, 21, false)]
        [InlineData(0, 0, 22, false)]
        [InlineData(0, 0, 23, false)]
        [InlineData(0, 0, 24, false)]
        [InlineData(0, 0, 26, false)]
        [InlineData(0, 0, 27, false)]
        [InlineData(0, 0, 28, false)]
        [InlineData(0, 0, 29, false)]
        [InlineData(0, 0, 31, false)]
        [InlineData(0, 0, 49, false)]
        [InlineData(0, 0, 51, false)]
        [InlineData(0, 0, 52, false)]
        [InlineData(0, 0, 53, false)]
        [InlineData(0, 0, 54, false)]
        [InlineData(0, 0, 56, false)]
        [InlineData(0, 0, 57, false)]
        [InlineData(0, 0, 58, false)]
        [InlineData(0, 0, 59, false)]
        [InlineData(5, 6, 54, false)]
        [InlineData(5, 6, 33, false)]
        [InlineData(2, 2, 1, false)]
        public async Task ValidEveryFiveMinutes(int day, int hour, int minute, bool shouldRun)
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            bool taskRan = false;

            scheduler.Schedule(() => taskRan = true).EveryFiveMinutes();

            await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);

            Assert.Equal(shouldRun, taskRan);
        }
    }
}