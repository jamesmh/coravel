using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling
{
    [TestClass]
    public class SchedulerHourlyAtTests
    {
        [TestMethod]
        [DataRow(-1, 60)] 
        [DataRow(10, 120)]
        [DataRow(60, 121)]
        [DataRow(0, 63)]
        public async Task ValidHourly_RunOnceOnTheHour(int first, int second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).HourlyAt(0);

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);

            Assert.IsTrue(taskRunCount == 1);
        }

        [TestMethod]
        [DataRow(0, 83)] 
        [DataRow(10, 83)]
        [DataRow(60, 143)]
        public async Task ValidHourly_RunOnceAtMin23(int first, int second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).HourlyAt(23);

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);

            Assert.IsTrue(taskRunCount == 1);
        }

        [TestMethod]
        [DataRow(1, 0, 70)] 
        [DataRow(5, 0, 66)]
        [DataRow(2, 0, 63)]
        [DataRow(2, 0, 61)]
        public async Task HourlyAt_ShouldNotExecuteScheduleTasks(int runAt, int first, int second)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).HourlyAt(runAt);

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);

            Assert.IsTrue(taskRunCount == 0);
        }

        [TestMethod]
        [DataRow(0, 25, 30, 60)]
        [DataRow(5, 55, 64, 70)]

        public async Task ValidHourlyAt_OneRun(int first, int second, int third, int fourth)
        {
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.Schedule(() => taskRunCount++).Hourly();

            await RunScheduledTasksFromMinutes(scheduler, first);
            await RunScheduledTasksFromMinutes(scheduler, second);

            Assert.IsTrue(taskRunCount == 1);
        }
    }
}