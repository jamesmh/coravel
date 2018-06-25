using System;
using System.Threading.Tasks;
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
        public async Task ValidEveryFifteenMinutes(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryFifteenMinutes();

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);
            await RunScheduledTasksFromMinutes(scheduler, third);
            await RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.IsTrue(taskRunCount == 4);
        }

        [TestMethod]
        [DataRow(0, 5, 14, 59)]
        [DataRow(15, 20, 29, 30)]
        [DataRow(30, 44, 45, 59)]

        public async Task ValidEveryFifteenMinutes_2RunsOnly(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryFifteenMinutes();

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);
            await RunScheduledTasksFromMinutes(scheduler, third);
            await RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}