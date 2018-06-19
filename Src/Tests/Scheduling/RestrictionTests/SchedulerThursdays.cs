using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.RestrictionTests
{
    [TestClass]
    public class SchedulerThursdays
    {
        [TestMethod]
        [DataTestMethod]
        public void EveryDailyOnThursdaysOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Thursday();

            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/06")); 
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/07")); //Thursday
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/08"));
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/13")); 
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/14")); //Thursday
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/15")); 

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}