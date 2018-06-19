using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling
{
    [TestClass]
    public class SchedulerEveryTenMinuteTests
    {
        [TestMethod]
        [DataRow(0, 10, 20, 30)]
        [DataRow(5, 15, 25, 35)]
        [DataRow(6, 16, 27, 59)]
        [DataRow(10, 21, 32, 42)]
        public void ValidEveryTenMinutes(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryTenMinutes();

            RunScheduledTasksFromMinutes(scheduler, first);
            RunScheduledTasksFromMinutes(scheduler, second);
            RunScheduledTasksFromMinutes(scheduler, third);
            RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.IsTrue(taskRunCount == 4);
        }

        [TestMethod]
        // Each case ought to run twice.
        [DataRow(0, 5, 14, 23)]
        [DataRow(15, 20, 26, 30)]

        public void ValidEveryTenMinutes_2RunsOnly(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryTenMinutes();

            RunScheduledTasksFromMinutes(scheduler, first);
            RunScheduledTasksFromMinutes(scheduler, second);
            RunScheduledTasksFromMinutes(scheduler, third);
            RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}