using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling
{
    [TestClass]
    public class SchedulerHourlyTests
    {
        [TestMethod]
        [DataRow(0, 60)]
        [DataRow(6, 66)]
        [DataRow(6, 67)]
        public void ValidHourly(int first, int second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).Hourly();

            RunScheduledTasksFromMinutes(scheduler, first);
            RunScheduledTasksFromMinutes(scheduler, second);

            Assert.IsTrue(taskRunCount == 2);
        }

        [TestMethod]
        [DataRow(0, 25, 30, 60)]
        [DataRow(5, 55, 64, 70)]

        public void ValidHourly_OneRun(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).Hourly();

            RunScheduledTasksFromMinutes(scheduler, first);
            RunScheduledTasksFromMinutes(scheduler, second);

            Assert.IsTrue(taskRunCount == 1);
        }
    }
}