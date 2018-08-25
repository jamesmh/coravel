using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;

namespace UnitTests.Scheduling.OverlappingTests
{
    public class OverlappingTests
    {
        [Fact]
        public async Task LongRunningEventPreventOverlap()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub());
            var semaphore = new SemaphoreSlim(0);
            int taskCount = 0;

            scheduler.Schedule(async () =>
            {
                await semaphore.WaitAsync(); // Simulate that this event takes a really long time.
                taskCount++;
            })
            .EveryMinute()
            .PreventOverlapping("PreventOverlappingTest");

            // Start the event (which will never complete until we manually release it)
            await scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:00 am"));
            Assert.Equal(0, taskCount);

            // Attempt to run the event again when it is still running.
            // This should prevent overlapping events.
            await scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:01 am"));
            Assert.Equal(0, taskCount);


            await scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:02 am"));
            Assert.Equal(0, taskCount);

            await scheduler.RunAtAsync(DateTime.Parse("2018/01/02 00:02 am"));
            Assert.Equal(0, taskCount);

            semaphore.Release();
            await Task.Delay(10); // Let the scheduled task complete asyncronously.

            // We should have only ever executed the scheduled task once.
            Assert.Equal(1, taskCount);
        }

        [Fact]
        public async Task OverlapNotPrevented()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub());
            int taskCount = 0;

            scheduler.Schedule(() =>
             {
                 taskCount++;
             })
             .EveryMinute();

            await scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:01 am"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/01/02 00:02 am"));
            await scheduler.RunAtAsync(DateTime.Parse("2018/01/03 00:03 am"));

            Assert.Equal(3, taskCount);
        }
    }
}