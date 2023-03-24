using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling.RestrictionTests
{
    public class SchedulerMondays
    {
        [Fact]
        public async Task DailyOnMondaysOnly()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Monday();

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/04")); //Monday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/05"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/06"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/11")); //Monday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/12"));

            Assert.True(taskRunCount == 2);
        }
    }
}