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
        public void DailyOnThursdaysOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Thursday();

            scheduler.RunAt(DateTime.Parse("2018/06/06")); 
            scheduler.RunAt(DateTime.Parse("2018/06/07")); //Thursday
            scheduler.RunAt(DateTime.Parse("2018/06/08"));
            scheduler.RunAt(DateTime.Parse("2018/06/13")); 
            scheduler.RunAt(DateTime.Parse("2018/06/14")); //Thursday
            scheduler.RunAt(DateTime.Parse("2018/06/15")); 

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}