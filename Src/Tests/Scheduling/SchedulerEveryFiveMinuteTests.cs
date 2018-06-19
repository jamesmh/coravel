using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling
{
    [TestClass]
    public class SchedulerEveryFiveMinuteTests
    {
        [TestMethod]
        [DataRow(0, 5, 10, 15)]
        [DataRow(20, 25, 30, 35)]
        [DataRow(4, 10, 21, 50)]
        [DataRow(30, 37, 50, 59)]
        public void ValidEveryFiveMinutes(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryFiveMinutes();

            RunScheduledTasksFromMinutes(scheduler, first);
            RunScheduledTasksFromMinutes(scheduler, second);
            RunScheduledTasksFromMinutes(scheduler, third);
            RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.IsTrue(taskRunCount == 4);
        }

        [TestMethod]
        // Each case ought to run twice.
        [DataRow(0, 3, 4, 5)]
        [DataRow(15, 16, 17, 30)]


        public void ValidEveryFiveMinutes_2RunsOnly(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryFiveMinutes();

            RunScheduledTasksFromMinutes(scheduler, first);
            RunScheduledTasksFromMinutes(scheduler, second);
            RunScheduledTasksFromMinutes(scheduler, third);
            RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}