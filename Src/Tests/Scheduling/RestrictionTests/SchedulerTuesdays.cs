using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.RestrictionTests
{
    [TestClass]
    public class SchedulerTuesdays
    {
        [TestMethod]
        [DataTestMethod]
        public void EveryDailyOnTuesdaysOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Tuesday();

            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/05")); //Tuesday
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/06"));
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/07"));
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/12")); //Tuesday
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/13"));

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}