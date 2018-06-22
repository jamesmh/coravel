using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.RestrictionTests
{
    [TestClass]
    public class SchedulerFridays
    {
        [TestMethod]
        [DataTestMethod]
        public void DailyOnFridaysOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Friday();

            scheduler.RunAt(DateTime.Parse("2018/06/07")); 
            scheduler.RunAt(DateTime.Parse("2018/06/08")); //Friday
            scheduler.RunAt(DateTime.Parse("2018/06/09"));
            scheduler.RunAt(DateTime.Parse("2018/06/14")); 
            scheduler.RunAt(DateTime.Parse("2018/06/15")); //Friday
            scheduler.RunAt(DateTime.Parse("2018/06/16")); 

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}