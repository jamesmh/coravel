using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.RestrictionTests
{
    public class SchedulerTuesdays
    {
        [Fact]
        public async Task DailyOnTuesdaysOnly()
        {
            var scheduler = new Scheduler(new InMemoryMutex());
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Tuesday();

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/05")); //Tuesday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/06"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/07"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/12")); //Tuesday
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/13"));

            Assert.True(taskRunCount == 2);
        }
    }
}