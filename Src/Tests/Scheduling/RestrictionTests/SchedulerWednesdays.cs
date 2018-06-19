using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.RestrictionTests
{
    [TestClass]
    public class SchedulerWednesdays
    {
        [TestMethod]
        [DataTestMethod]
        public void DailyOnWednesdayOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Wednesday();

            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/05")); 
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/06")); //Wednesday
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/07"));
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/12")); 
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/13")); //Wednesday
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/14")); 

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}