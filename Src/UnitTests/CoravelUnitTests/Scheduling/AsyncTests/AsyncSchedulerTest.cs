using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;
using static CoravelUnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace CoravelUnitTests.Scheduling.AsyncTests
{
    public class AsyncSchedulerTest
    {
        [Fact]
        public async Task AsyncSchedulesTest()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            int taskRunCount = 0;

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

            await RunScheduledTasksFromMinutes(scheduler, 0);

            Assert.Equal(2, taskRunCount);
        }
    }
}