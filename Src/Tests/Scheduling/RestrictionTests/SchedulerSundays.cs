using System;
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
        public void DailyOnSundaysOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Sunday();

            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/09")); 
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/10")); //Sunday
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/11"));
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/16")); 
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/17")); //Sunday
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/18")); 

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}