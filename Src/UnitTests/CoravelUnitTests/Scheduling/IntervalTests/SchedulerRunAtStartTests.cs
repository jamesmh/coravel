using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;
using static CoravelUnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace CoravelUnitTests.Scheduling.IntervalTests
{
    public class SchedulerRunAtStartTests
    {
        [Theory]
        // Normally should not run
        [InlineData(6, 59, 4, 6, 58)]
        [InlineData(6, 59, 4, 7, 0)]
        [InlineData(6, 59, 6, 6, 12)]
        [InlineData(6, 59, 6, 7, 0)]
        [InlineData(6, 59, 1, 10, 59)]
        [InlineData(6, 59, 1, 7, 0)]
        [InlineData(6, 59, 1, 6, 58)]
        [InlineData(6, 59, 1, 6, 57)]
        [InlineData(0, 0, 4, 7, 0)]
        [InlineData(0, 0, 0, 0, 1)]
        [InlineData(0, 1, 0, 0, 0)]
        [InlineData(0, 0, 2, 23, 58)]
        [InlineData(0, 0, 2, 23, 59)]
        public async Task TestRunsOnlyAtStart(int atHour, int atMinute, int day, int hour, int minute)
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            var taskRan = false;

            scheduler
                .Schedule(() => taskRan = true)
                .DailyAt(atHour, atMinute)
                .RunOnceAtStart();

            await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);
            Assert.True(taskRan);

            taskRan = false;
            await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);

            Assert.False(taskRan);
        }

        [Theory]
        // Normally should run
        [InlineData(0, 0, 0, 0, 0)]
        [InlineData(0, 0, 4, 0, 0)]
        [InlineData(0, 32, 4, 0, 32)]
        [InlineData(0, 32, 6, 0, 32)]
        [InlineData(13, 2, 4, 13, 2)]
        [InlineData(13, 2, 2, 13, 2)]
        [InlineData(6, 59, 4, 6, 59)]
        [InlineData(6, 59, 6, 6, 59)]
        public async Task TestRunsAtStartAndWhenDue(int atHour, int atMinute, int day, int hour, int minute)
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            var taskRan = false;

            scheduler
                .Schedule(() => taskRan = true)
                .DailyAt(atHour, atMinute)
                .RunOnceAtStart();

            await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);
            Assert.True(taskRan);

            taskRan = false;
            await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);

            Assert.True(taskRan);
        }
    }
}