using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Xunit;
using static CoravelUnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace CoravelUnitTests.Scheduling.RestrictionTests;

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
