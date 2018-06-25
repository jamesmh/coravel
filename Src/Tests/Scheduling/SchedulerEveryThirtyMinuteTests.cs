using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

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
        public async Task ValidEveryThirtyMinutes(int first, int second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryThirtyMinutes();

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);

            Assert.IsTrue(taskRunCount == 2);
        }

        [TestMethod]
        [DataRow(0, 25, 30, 59)]
        [DataRow(5, 20, 34, 36)]

        public async Task ValidEveryThirtyMinutes_2RunsOnly(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).EveryThirtyMinutes();

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);
            await RunScheduledTasksFromMinutes(scheduler, third);
            await RunScheduledTasksFromMinutes(scheduler, fourth);

            Assert.IsTrue(taskRunCount == 2);
        }
    }
}