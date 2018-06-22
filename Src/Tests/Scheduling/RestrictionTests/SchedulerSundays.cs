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

            scheduler.RunAt(DateTime.Parse("2018/06/09")); 
            scheduler.RunAt(DateTime.Parse("2018/06/10")); //Sunday
            scheduler.RunAt(DateTime.Parse("2018/06/11"));
            scheduler.RunAt(DateTime.Parse("2018/06/16")); 
            scheduler.RunAt(DateTime.Parse("2018/06/17")); //Sunday
            scheduler.RunAt(DateTime.Parse("2018/06/18")); 

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}