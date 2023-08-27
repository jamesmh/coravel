using System;
using System.Globalization;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling.IntervalTests;

public class SchedulerWeeklyTests
{
    [Theory]
    // Should run
    [InlineData("2018-7-30 00:00:00 am", true)]
    [InlineData("2018-8-6 00:00:00 am", true)]
    [InlineData("2018-8-13 00:00:00 am", true)]
    [InlineData("2018-8-20 00:00:00 am", true)]
    [InlineData("2018-8-27 00:00:00 am", true)]
    [InlineData("2018-9-3 00:00:00 am", true)]
    [InlineData("2018-9-10 00:00:00 am", true)]
    [InlineData("2018-9-17 00:00:00 am", true)]
    [InlineData("2018-9-24 00:00:00 am", true)]
    // Should not run
    [InlineData("2018-7-31 00:00:00 am", false)]
    [InlineData("2018-8-1 00:00:00 am", false)]
    [InlineData("2018-8-2 00:00:00 am", false)]
    [InlineData("2018-8-3 00:00:00 am", false)]
    [InlineData("2018-8-4 00:00:00 am", false)]
    [InlineData("2018-8-5 00:00:00 am", false)]
    [InlineData("2018-8-6 00:01:00 am", false)]
    [InlineData("2018-8-5 12:59:59 pm", false)]
    public async Task ValidDaily(string dateString, bool shouldRun)
    {
        var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
        bool taskRan = false;

        scheduler.Schedule(() => taskRan = true).Weekly();
        await scheduler.RunAtAsync(DateTime.Parse(dateString, new CultureInfo("en-US")));
        Assert.Equal(shouldRun, taskRan);
    }
}