using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.SchedulingTestHelpers;

namespace Tests.Scheduling
{
    [TestClass]
    public class SchedulerEveryFixMinuteTests
    {
        [TestMethod]
        // These are exactly 15 min apart
        [DataRow(0, 15, 30, 45)]
        [DataRow(5, 20, 35, 50)]
        [DataRow(6, 21, 36, 51)]
        // These have gaps of over 15 min to test if they still run
        [DataRow(5, 20, 40, 59)]
        [DataRow(0, 17, 40, 59)]
        public void ValidEveryFiveMinutes(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryMinute();

            RunScheduledTasksFromMinutes(scheduler, first);
            RunScheduledTasksFromMinutes(scheduler, second);
            RunScheduledTasksFromMinutes(scheduler, third);
            RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.IsTrue(taskRunCount == 4);
        }

        [TestMethod]
        // Each case has two intervals that ought to be executed.
        [DataRow(0, 5, 16, 59)]
        [DataRow(0, 15, 20, 30)]
        [DataRow(10, 20, 29, 59)]

        public void ValidEveryFiveMinutes_2RunsOnly(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryMinute();

            RunScheduledTasksFromMinutes(scheduler, first);
            RunScheduledTasksFromMinutes(scheduler, second);
            RunScheduledTasksFromMinutes(scheduler, third);
            RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.IsTrue(taskRunCount == 4);
        }
    }
}