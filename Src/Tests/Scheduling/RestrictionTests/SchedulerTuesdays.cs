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
        public void DailyOnTuesdaysOnly() {
              var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Tuesday();

            scheduler.RunAt(DateTime.Parse("2018/06/05")); //Tuesday
            scheduler.RunAt(DateTime.Parse("2018/06/06"));
            scheduler.RunAt(DateTime.Parse("2018/06/07"));
            scheduler.RunAt(DateTime.Parse("2018/06/12")); //Tuesday
            scheduler.RunAt(DateTime.Parse("2018/06/13"));

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}