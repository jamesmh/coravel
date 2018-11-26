using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.RestrictionTests
{
    public class SchedulerPredicate {

        [Fact]
        public async Task PredicateTest()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            int taskRunCount = 0;

            async Task<bool> filterAsyncFail()
            {
                await Task.Delay(0);
                return false;
            }

            async Task<bool> filterAsyncPass()
            {
                await Task.Delay(0);
                return true;
            }

            scheduler.Schedule(() =>
            {
                taskRunCount++;
            }).EveryMinute().When(filterAsyncFail);

            scheduler.Schedule(() =>
            {
                taskRunCount++;
            }).EveryMinute().When(filterAsyncPass);
          
            scheduler.Schedule(() =>
            {
                taskRunCount++;
            }).EveryMinute().When(filterAsyncFail);

            scheduler.Schedule(() =>
            {
                taskRunCount++;
            }).EveryMinute().When(filterAsyncPass);

            scheduler.Schedule(() =>
            {
                taskRunCount++;
            }).EveryMinute().When(filterAsyncFail);

            await RunScheduledTasksFromMinutes(scheduler, 0);

            Assert.Equal(2, taskRunCount);
        }
    }
}
