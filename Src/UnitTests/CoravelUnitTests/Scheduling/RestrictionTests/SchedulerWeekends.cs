using System;
using System.Globalization;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling.RestrictionTests;

public class SchedulerWeekends
{
    [Fact]
    public async Task DailyOnWeekendsOnly()
    {
        var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
        int taskRunCount = 0;

        scheduler.Schedule(() => taskRunCount++)
        .Daily()
        .Weekend();

        await scheduler.RunAtAsync(DateTime.Parse("2018/06/09", new CultureInfo("en-US"))); //Sat
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/10", new CultureInfo("en-US"))); //Sun
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/11", new CultureInfo("en-US"))); //Mon
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/12", new CultureInfo("en-US"))); //Tue
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/13", new CultureInfo("en-US"))); //W
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/14", new CultureInfo("en-US"))); //T
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/15", new CultureInfo("en-US"))); //F
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/16", new CultureInfo("en-US"))); //S
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/17", new CultureInfo("en-US"))); //S
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/18", new CultureInfo("en-US"))); //M

        Assert.True(taskRunCount == 4);
    }
}