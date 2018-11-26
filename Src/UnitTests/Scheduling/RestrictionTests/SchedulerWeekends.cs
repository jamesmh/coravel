using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.RestrictionTests
{
    public class SchedulerWeekends
    {
        [Fact]
        public async Task DailyOnWeekendsOnly()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Weekend();

            await scheduler.RunAtAsync(DateTime.Parse("2018/06/09")); //Sat
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/10")); //Sun
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/11")); //Mon
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/12")); //Tue
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/13")); //W
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/14")); //T
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/15")); //F
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/16")); //S
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/17")); //S
            await scheduler.RunAtAsync(DateTime.Parse("2018/06/18")); //M

            Assert.True(taskRunCount == 4);
        }
    }
}