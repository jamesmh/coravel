using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.IntervalTests
{
    public class SchedulerDailyTests
    {
        [Theory]
        // Should run
        [InlineData(0, 0, 0, true)]
        [InlineData(1, 0, 0, true)]
        [InlineData(2, 0, 0, true)]
        [InlineData(3, 0, 0, true)]
        [InlineData(4, 0, 0, true)]
        [InlineData(5, 0, 0, true)]
        [InlineData(6, 0, 0, true)]
        // Should not run
        [InlineData(0, 0, 1, false)]
        [InlineData(0, 0, 2, false)]
        [InlineData(0, 1, 0, false)]
        [InlineData(0, 1, 59, false)]
        [InlineData(4, 1, 0, false)]
        [InlineData(4, 23, 59, false)]
        [InlineData(5, 0, 1, false)]

        public async Task ValidDaily(int day, int hour, int minute, bool shouldRun)
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            bool taskRan = false;

            scheduler.Schedule(() => taskRan = true).Daily();

            await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);

            Assert.Equal(shouldRun, taskRan);
        }
    }
}