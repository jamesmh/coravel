using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.RestrictionTests
{
    [TestClass]
    public class SchedulerWeekdays
    {
        [TestMethod]
        [DataTestMethod]
        public void DailyOnWeekdaysOnly()
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++)
            .Daily()
            .Weekday();

            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/09")); //Sat
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/10")); //Sun
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/11")); //Mon
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/12")); //Tue
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/13")); //W
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/14")); //T
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/15")); //F
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/16")); //S
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/17")); //S
            scheduler.RunScheduledTasks(DateTime.Parse("2018/06/18")); //M

            Assert.IsTrue(taskRunCount == 6);
        }
    }
}