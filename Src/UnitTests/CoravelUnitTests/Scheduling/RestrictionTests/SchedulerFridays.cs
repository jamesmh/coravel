using System;
using System.Globalization;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling.RestrictionTests;

public class SchedulerFridays
{
    [Fact]
    public async Task DailyOnFridaysOnly()
    {
        var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
        int taskRunCount = 0;

        scheduler.Schedule(() => taskRunCount++)
        .Daily()
        .Friday();

        await scheduler.RunAtAsync(DateTime.Parse("2018/06/07", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/08", new CultureInfo("en-US"))); //Friday
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/09", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/14", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/15", new CultureInfo("en-US"))); //Friday
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/16", new CultureInfo("en-US")));

        Assert.True(taskRunCount == 2);
    }
}