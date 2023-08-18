using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling.RestrictionTests
{
    public class SchedulerSaturdays
    {
        [Fact]
        public async Task DailyOnSaturdaysOnly()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Saturday();

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/08"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/09")); //Saturday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/10"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/15"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/16")); //Saturday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/17"));

            Assert.True(taskRunCount == 2);
        }
    }
}