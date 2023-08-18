using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling.RestrictionTests
{
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

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/07"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/08")); //Friday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/09"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/14"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/15")); //Friday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/16"));

            Assert.True(taskRunCount == 2);
        }
    }
}