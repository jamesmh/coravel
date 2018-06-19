using System;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling
{
    [TestClass]
    public class SchedulerDailyTests
    {
        [TestMethod]
        // Note: arrays are [day, hours, minutes]
        [DataRow(new int[] { 0, 0, 0 }, new int[] { 1, 0, 0 })]
        [DataRow(new int[] { 0, 0, 0 }, new int[] { 1, 1, 0 })]
        [DataRow(new int[] { 0, 6, 0 }, new int[] { 1, 7, 0 })]
        [DataRow(new int[] { 0, 1, 0 }, new int[] { 1, 1, 0 })]
        [DataRow(new int[] { 0, 0, 5 }, new int[] { 1, 0, 5 })]
        [DataRow(new int[] { 0, 0, 5 }, new int[] { 1, 0, 55 })]
        public void ValidDaily(int[] first, int[] second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).Daily();

            RunScheduledTasksFromDayHourMinutes(scheduler, first[0], first[1], first[2]);
            RunScheduledTasksFromDayHourMinutes(scheduler, second[0], second[1], second[2]);

            Assert.IsTrue(taskRunCount == 2);
        }

                [TestMethod]
        // Note: arrays are [day, hours, minutes]
        [DataRow(new int[] { 0, 0, 0 }, new int[] { 0, 23, 59 })]
        [DataRow(new int[] { 0, 5, 0 }, new int[] { 1, 4, 0 })]
        [DataRow(new int[] { 0, 5, 0 }, new int[] { 1, 4, 59 })]
        public void Daily_ShouldRunOnce(int[] first, int[] second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).Daily();

            RunScheduledTasksFromDayHourMinutes(scheduler, first[0], first[1], first[2]);
            RunScheduledTasksFromDayHourMinutes(scheduler, second[0], second[1], second[2]);

            Assert.IsTrue(taskRunCount == 1);
        }
    }
}