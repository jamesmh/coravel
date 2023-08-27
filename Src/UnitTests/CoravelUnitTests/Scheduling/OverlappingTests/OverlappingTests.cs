using System;
using System.Globalization;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling.OverlappingTests;

public class OverlappingTests
{
    [Fact]
    public async Task LongRunningEventPreventOverlap()
    {
        var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
        int taskCount = 0;

        scheduler.ScheduleAsync(async () =>
        {
            await Task.Delay(200);
            // Simulate that this event takes a really long time.
            taskCount++;
        })
        .EveryMinute()
        .PreventOverlapping("PreventOverlappingTest");

        var longRunningTask = scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:00 am", new CultureInfo("en-US")));

        await Task.Delay(1); // Make sure above starts.

        await Task.WhenAll(
            scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:01 am", new CultureInfo("en-US"))),
            scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:02 am", new CultureInfo("en-US"))),
            scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:03 am", new CultureInfo("en-US")))
        );

        await longRunningTask;

        // We should have only ever executed the scheduled task once.
        Assert.Equal(1, taskCount);
    }

    [Fact]
    public async Task OverlapNotPrevented()
    {
        var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
        int taskCount = 0;

        scheduler.Schedule(() =>
         {
             taskCount++;
         })
         .EveryMinute();

        await scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:01 am", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/01/02 00:02 am", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/01/03 00:03 am", new CultureInfo("en-US")));

        Assert.Equal(3, taskCount);
    }
}