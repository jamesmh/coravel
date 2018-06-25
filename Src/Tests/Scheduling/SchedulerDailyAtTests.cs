using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling
{
    [TestClass]
    public class SchedulerDailyAtTests
    {
        [TestMethod]
        // Note: arrays are [day, hours, minutes]
        [DataRow(new int[] { 0, 0, 0 }, new int[] { 1, 13, 0 })]
        [DataRow(new int[] { 0, 5, 59 }, new int[] { 1, 13, 0 })]
        [DataRow(new int[] { 0, 6, 4 }, new int[] { 5, 13, 0 })]
        public async Task ValidDailyAt_1pm(int[] first, int[] second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).DailyAt(13, 0);

            await RunScheduledTasksFromDayHourMinutes(scheduler, first[0], first[1], first[2]);
            await RunScheduledTasksFromDayHourMinutes(scheduler, second[0], second[1], second[2]);

            Assert.IsTrue(taskRunCount == 1);
        }

        [TestMethod]
        // Note: arrays are [day, hours, minutes]
        [DataRow(new int[] { 0, 0, 0 }, new int[] { 0, 0, 34 })]
        [DataRow(new int[] { 0, 5, 59 }, new int[] { 2, 0, 34 })]
        [DataRow(new int[] { 0, 6, 4 }, new int[] { 5, 0, 34 })]
        public async Task ValidDailyAt_34MinAfterMidnight(int[] first, int[] second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).DailyAt(00, 34);

            await RunScheduledTasksFromDayHourMinutes(scheduler, first[0], first[1], first[2]);
            await RunScheduledTasksFromDayHourMinutes(scheduler, second[0], second[1], second[2]);

            Assert.IsTrue(taskRunCount == 1);
        }

        [TestMethod]
        // Note: arrays are [day, hours, minutes]
        [DataRow(new int[] { 0, 15, 01 }, new int[] { 2, 15, 02 })]
        [DataRow(new int[] { 0, 14, 59 }, new int[] { 2, 14, 59 })]
        [DataRow(new int[] { 0, 6, 4 }, new int[] { 5, 14, 00 })]
        public async Task DailyAt_3pm_ShouldNeverRun(int[] first, int[] second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).DailyAt(00, 34);

            await RunScheduledTasksFromDayHourMinutes(scheduler, first[0], first[1], first[2]);
            await RunScheduledTasksFromDayHourMinutes(scheduler, second[0], second[1], second[2]);

            Assert.IsTrue(taskRunCount == 0);
        }
    }
}