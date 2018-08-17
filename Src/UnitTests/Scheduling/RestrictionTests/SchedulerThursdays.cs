using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.RestrictionTests
{
    public class SchedulerThursdays
    {
        [Fact]
        public async Task DailyOnThursdaysOnly()
        {
            var scheduler = new Scheduler(new InMemoryMutex());
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Thursday();

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/06"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/07")); //Thursday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/08"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/13"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/14")); //Thursday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/15"));

            Assert.True(taskRunCount == 2);
        }
    }
}