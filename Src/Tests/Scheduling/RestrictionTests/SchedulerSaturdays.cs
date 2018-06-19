using System;
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
        public void DailyOnSaturdaysOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Saturday();

            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/08")); 
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/09")); //Saturday
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/10"));
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/15")); 
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/16")); //Saturday
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/17")); 

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}