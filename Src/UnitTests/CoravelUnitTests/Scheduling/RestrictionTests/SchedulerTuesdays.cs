using System;
using System.Globalization;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling.RestrictionTests;

public class SchedulerTuesdays
{
    [Fact]
    public async Task DailyOnTuesdaysOnly()
    {
        var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
        int taskRunCount = 0;

        scheduler.Schedule(() => taskRunCount++)
        .Daily()
        .Tuesday();

        await scheduler.RunAtAsync(DateTime.Parse("2018/06/05", new CultureInfo("en-US"))); //Tuesday
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/06", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/07", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/12", new CultureInfo("en-US"))); //Tuesday
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/13", new CultureInfo("en-US")));

        Assert.True(taskRunCount == 2);
    }
}