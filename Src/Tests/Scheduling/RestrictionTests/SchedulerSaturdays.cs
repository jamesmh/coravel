using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.RestrictionTests
{
    [TestClass]
    public class SchedulerSaturdays
    {
        [TestMethod]
        [DataTestMethod]
        public async Task DailyOnSaturdaysOnly() {
              var scheduler = new Scheduler();
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

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}