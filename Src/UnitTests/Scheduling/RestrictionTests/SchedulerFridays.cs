using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.RestrictionTests
{
    public class SchedulerFridays
    {
        [Fact]
        public async Task DailyOnFridaysOnly()
        {
            var scheduler = new Scheduler(new InMemoryMutex());
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