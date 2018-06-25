using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.RestrictionTests
{
    [TestClass]
    public class SchedulerSundays
    {
        [TestMethod]
        [DataTestMethod]
        public async Task DailyOnSundaysOnly() {
              var scheduler = new Scheduler();
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

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}