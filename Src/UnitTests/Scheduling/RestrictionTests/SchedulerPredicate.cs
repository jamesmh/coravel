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
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub());
            int taskRunCount = 0;

            async Task<bool> filterAsyncFail()
            {
                await Task.Delay(5000);
                return false;
            }

            async Task<bool> filterAsyncPass()
            {
                await Task.Delay(5000);
                return true;
            }

            bool filter()
            {
                return taskRunCount < 2;
            }

            scheduler.Schedule(() =>
            {
                taskRunCount++;
            }).EveryMinute().When(filter);

            scheduler.Schedule(() =>
            {
                taskRunCount++;
            }).EveryMinute().When(filter);

            scheduler.Schedule(() =>
            {
                taskRunCount++;
            }).EveryMinute().When(filter);

            scheduler.Schedule(() =>
            {
                taskRunCount++;
            }).EveryMinute().When(filterAsyncPass());

            scheduler.Schedule(() =>
            {
                taskRunCount++;
            }).EveryMinute().When(filterAsyncFail());

            await RunScheduledTasksFromMinutes(scheduler, 0);

            Assert.Equal(3, taskRunCount);
        }
    }
}
