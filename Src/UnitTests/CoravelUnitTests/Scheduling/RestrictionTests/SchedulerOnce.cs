using System;
using System.Globalization;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoravelUnitTests.Scheduling.RestrictionTests;

public class SchedulerOnce
{
    [Fact]
    public async Task ScheduleOnceWithCron()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IScheduler>(p => new Scheduler(new InMemoryMutex(),
            p.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub()));
        var provider = services.BuildServiceProvider();
        var scheduler = provider.GetRequiredService<IScheduler>() as Scheduler;
        
        int taskRunCount = 0;

        scheduler!.Schedule(() => taskRunCount++)
            .Cron("* * * * *")
            .Once();

        await scheduler.RunAtAsync(DateTime.Parse("2018/06/09", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/10", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/11", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/12", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/13", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/14", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/15", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/16", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/17", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/18", new CultureInfo("en-US")));

        Assert.True(taskRunCount == 1);
    }
    
    [Fact]
    public async Task ScheduleOnceAtHourly()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IScheduler>(p => new Scheduler(new InMemoryMutex(),
            p.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub()));
        var provider = services.BuildServiceProvider();
        var scheduler = provider.GetRequiredService<IScheduler>() as Scheduler;
        
        int taskRunCount = 0;

        scheduler!.Schedule(() => taskRunCount++)
           .Hourly()
            .Once();
        
        // Shouldn't be due here.
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/09 12:00:01 pm", new CultureInfo("en-US")));
        Assert.True(taskRunCount == 0);

        await scheduler.RunAtAsync(DateTime.Parse("2018/06/09 12:00:00 pm", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/10 5:00:00 am", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/09 1:00:00 am", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/10 2:00:00 pm", new CultureInfo("en-US")));

        Assert.True(taskRunCount == 1);
    }
}