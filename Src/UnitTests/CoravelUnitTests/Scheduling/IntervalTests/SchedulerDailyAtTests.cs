using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;
using static CoravelUnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace CoravelUnitTests.Scheduling.IntervalTests;

public class SchedulerDailyAtTests
{
    [Theory]
    // Should Run
    [InlineData(0, 0, 0, 0, 0, true)]
    [InlineData(0, 0, 4, 0, 0, true)]
    [InlineData(0, 32, 4, 0, 32, true)]
    [InlineData(0, 32, 6, 0, 32, true)]
    [InlineData(13, 2, 4, 13, 2, true)]
    [InlineData(13, 2, 2, 13, 2, true)]
    [InlineData(6, 59, 4, 6, 59, true)]
    [InlineData(6, 59, 6, 6, 59, true)]
    // Should not run
    [InlineData(6, 59, 4, 6, 58, false)]
    [InlineData(6, 59, 4, 7, 0, false)]
    [InlineData(6, 59, 6, 6, 12, false)]
    [InlineData(6, 59, 6, 7, 0, false)]
    [InlineData(6, 59, 1, 10, 59, false)]
    [InlineData(6, 59, 1, 7, 0, false)]
    [InlineData(6, 59, 1, 6, 58, false)]
    [InlineData(6, 59, 1, 6, 57, false)]
    [InlineData(0, 0, 4, 7, 0, false)]
    [InlineData(0, 0, 0, 0, 1, false)]
    [InlineData(0, 1, 0, 0, 0, false)]
    [InlineData(0, 0, 2, 23, 58, false)]
    [InlineData(0, 0, 2, 23, 59, false)]

    public async Task DailyTests(int atHour, int atMinute, int day, int hour, int minute, bool shouldRun)
    {
        var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
        bool taskRan = false;

        scheduler.Schedule(() => taskRan = true).DailyAt(atHour, atMinute);

        await RunScheduledTasksFromDayHourMinutes(scheduler, day, hour, minute);

        Assert.Equal(shouldRun, taskRan);
    }
}