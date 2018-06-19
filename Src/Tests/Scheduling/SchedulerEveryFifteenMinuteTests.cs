using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling
{
    [TestClass]
    public class SchedulerEveryFifteenMinuteTests
    {
        [TestMethod]
        [DataRow(0, 15, 30, 45)]
        [DataRow(5, 20, 35, 50)]
        [DataRow(6, 21, 36, 51)]
        [DataRow(5, 20, 40, 59)]
        [DataRow(0, 17, 40, 59)]
        public void ValidEveryFifteenMinutes(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryFifteenMinutes();

            RunScheduledTasksFromMinutes(scheduler, first);
            RunScheduledTasksFromMinutes(scheduler, second);
            RunScheduledTasksFromMinutes(scheduler, third);
            RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.IsTrue(taskRunCount == 4);
        }

        [TestMethod]
        [DataRow(0, 5, 14, 59)]
        [DataRow(15, 20, 29, 30)]
        [DataRow(30, 44, 45, 59)]

        public void ValidEveryFifteenMinutes_2RunsOnly(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryFifteenMinutes();

            RunScheduledTasksFromMinutes(scheduler, first);
            RunScheduledTasksFromMinutes(scheduler, second);
            RunScheduledTasksFromMinutes(scheduler, third);
            RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}