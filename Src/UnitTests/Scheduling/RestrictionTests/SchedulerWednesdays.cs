using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.RestrictionTests
{
    public class SchedulerWednesdays
    {
        [Fact]
        public async Task DailyOnWednesdayOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Wednesday();

           await scheduler.RunAtAsync(DateTime.Parse("2018/06/05")); 
           await scheduler.RunAtAsync(DateTime.Parse("2018/06/06")); //Wednesday
           await scheduler.RunAtAsync(DateTime.Parse("2018/06/07"));
           await scheduler.RunAtAsync(DateTime.Parse("2018/06/12")); 
           await scheduler.RunAtAsync(DateTime.Parse("2018/06/13")); //Wednesday
           await scheduler.RunAtAsync(DateTime.Parse("2018/06/14")); 

            Assert.True(taskRunCount == 2);
        }
    }
}