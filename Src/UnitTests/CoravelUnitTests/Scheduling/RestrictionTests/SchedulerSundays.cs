using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling.RestrictionTests
{
    public class SchedulerSundays
    {
        [Fact]
        public async Task DailyOnSundaysOnly()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Sunday();

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/09"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/10")); //Sunday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/11"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/16"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/17")); //Sunday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/18"));

            Assert.True(taskRunCount == 2);
        }
    }
}