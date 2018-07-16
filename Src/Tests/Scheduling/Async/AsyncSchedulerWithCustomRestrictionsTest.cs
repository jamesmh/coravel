using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.Async
{
    [TestClass]
    public class AsyncSchedulerWithCustomRestrictionsTest
    {
        [TestMethod]
        public async Task AsyncSchedulesTest()
        {
            Contract.Ensures(Contract.Result<Task>() != null);
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            // Running async action synchrously to test warning
            scheduler.Schedule(async () =>
            {
                await Task.Delay(1);
            }).EveryMinute();

            scheduler.ScheduleAsync(async () =>
            {
                await Task.Delay(1);
                taskRunCount++;
            }).EveryMinute();

            scheduler.ScheduleAsync(async () =>
            {
                await Task.Delay(1);
                taskRunCount++;
            }).EveryMinute();

            scheduler.ScheduleAsync(async () =>
            {
                await Task.Delay(1);
                taskRunCount++;
            }).Where(() => taskRunCount < 2).EveryMinute();

            scheduler.ScheduleAsync(async () =>
            {
                await Task.Delay(1);
                taskRunCount++;
            }).Where (() => taskRunCount == 2).EveryMinute();

            scheduler.ScheduleAsync(async () =>
            {
                await Task.Delay(1);
                taskRunCount++;
            }).Where(() => taskRunCount == 3).EveryMinute();

            await RunScheduledTasksFromMinutes(scheduler, 0);

            Assert.AreEqual(4, taskRunCount);
        }
    }
}