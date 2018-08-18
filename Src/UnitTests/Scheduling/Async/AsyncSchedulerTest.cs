using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.Async
{
    public class AsyncSchedulerTest
    {
        [Fact]
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

            Assert.Equal(2, taskRunCount);
        }
    }
}