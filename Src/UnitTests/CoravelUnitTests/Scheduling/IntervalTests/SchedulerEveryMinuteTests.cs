using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;
using static CoravelUnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace CoravelUnitTests.Scheduling.IntervalTests
{
    public class SchedulerEveryMinuteTests
    {
        [Theory]
        // Should Run
        [InlineData(0, 0, 0, true)]
        [InlineData(0, 0, 1, true)]
        [InlineData(0, 0, 2, true)]
        [InlineData(0, 0, 3, true)]
        [InlineData(0, 0, 4, true)]
        [InlineData(0, 0, 6, true)]
        [InlineData(0, 0, 7, true)]
        [InlineData(0, 0, 8, true)]
        [InlineData(3, 5, 9, true)]
        [InlineData(3, 5, 11, true)]
        [InlineData(3, 5, 12, true)]
        [InlineData(3, 5, 13, true)]
        [InlineData(3, 5, 14, true)]
        [InlineData(6, 23, 16, true)]
        [InlineData(6, 23, 17, true)]
        [InlineData(6, 23, 18, true)]
        [InlineData(6, 23, 19, true)]
        [InlineData(6, 23, 21, true)]
        [InlineData(6, 23, 22, true)]
        [InlineData(6, 23, 23, true)]
        [InlineData(1, 14, 24, true)]
        [InlineData(1, 14, 26, true)]
        [InlineData(1, 14, 27, true)]
        [InlineData(1, 14, 28, true)]
        [InlineData(1, 14, 29, true)]
        [InlineData(1, 14, 31, true)]
        [InlineData(1, 14, 49, true)]
        [InlineData(1, 14, 51, true)]
        [InlineData(1, 14, 52, true)]
        [InlineData(1, 14, 53, true)]
        [InlineData(1, 14, 54, true)]
        [InlineData(4, 7, 56, true)]
        [InlineData(4, 7, 57, true)]
        [InlineData(4, 7, 58, true)]
        [InlineData(4, 7, 59, true)]
        [InlineData(5, 6, 54, true)]
        public async Task ValidEveryMinute(int day, int hour, int minute, bool shouldRun)
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            bool taskRan = false;

            scheduler.Schedule(() => taskRan = true).EveryMinute();

            await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);

            Assert.Equal(shouldRun, taskRan);
        }
    }
}