using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoravelUnitTests.Scheduling;

public class SchedulerZonedScheduleTests
{
    [Fact]
    public async Task SchedulerRunsZonedIntervals()
    {
        var services = new ServiceCollection();
        services.AddScoped<TestInvocable>();
        var provider = services.BuildServiceProvider();
        var scheduler = new Scheduler(new InMemoryMutex(), provider.GetRequiredService<IServiceScopeFactory>(), null);

        scheduler.Schedule<TestInvocable>()
        .EveryMinute()
        .Zoned(TimeZoneInfo.Local);

        await scheduler.RunAtAsync(DateTime.Parse("2018/06/07", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/08", new CultureInfo("en-US")));
        await scheduler.RunAtAsync(DateTime.Parse("2018/06/09", new CultureInfo("en-US")));

        Assert.Equal(3, TestInvocable.Test);
    }

    public class TestInvocable : IInvocable
    {
        public static int Test = 0;

        public Task Invoke()
        {
            Interlocked.Increment(ref Test);
            return Task.CompletedTask;
        }
    }
}