using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling.Async
{
    [TestClass]
    public class AsyncSchedulerTest
    {
        [TestMethod]
        public async Task AsyncSchedulesTest(){
            var scheduler = new Scheduler();
            int taskRunCount = 0;

            scheduler.ScheduleAsync(async () => {
                await Task.Delay(1);
                taskRunCount++;
            }).EveryMinute();

            scheduler.ScheduleAsync(async () => {
                await Task.Delay(1);
                taskRunCount++;
            }).EveryMinute();

            await RunScheduledTasksFromMinutes(scheduler, 0);

            Assert.AreEqual(2, taskRunCount);
        }
    }
}