using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.SchedulingTestHelpers;

namespace Tests.Scheduling
{
    [TestClass]
    public class SchedulerEveryThirtyMinuteTests
    {
        [TestMethod]
        [DataRow(0, 30)]
        [DataRow(6, 36)]
        [DataRow(6, 40)]
        [DataRow(10, 41)]
        public void ValidEveryThirtyMinutes(int first, int second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryThirtyMinutes();

            RunScheduledTasksFromMinutes(scheduler, first);
            RunScheduledTasksFromMinutes(scheduler, second);

            Assert.IsTrue(taskRunCount == 2);
        }

        [TestMethod]
        [DataRow(0, 25, 30, 59)]
        [DataRow(5, 20, 34, 36)]

        public void ValidEveryThirtyMinutes_2RunsOnly(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryThirtyMinutes();

            RunScheduledTasksFromMinutes(scheduler, first);
            RunScheduledTasksFromMinutes(scheduler, second);
            RunScheduledTasksFromMinutes(scheduler, third);
            RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}