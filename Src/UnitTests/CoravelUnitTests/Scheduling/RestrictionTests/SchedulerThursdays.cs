using System;
using System.Globalization;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling.RestrictionTests;

public class SchedulerThursdays
{
    [Fact]
    public async Task DailyOnThursdaysOnly()
    {
        var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
        int taskRunCount = 0;

        scheduler.Schedule(() => taskRunCount++)
        .Daily()
        .Thursday();

        await scheduler.RunAtAsync(DateTime.Parse("2018/06/06", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/07", new CultureInfo("en-US"))); //Thursday
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/08", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/13", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/14", new CultureInfo("en-US"))); //Thursday
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/15", new CultureInfo("en-US")));

        Assert.True(taskRunCount == 2);
    }
}