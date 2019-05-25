using System;
using System.Collections.Generic;
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
       // [Fact]
        public async Task LongRunningEventPreventOverlap()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            var semaphore = new SemaphoreSlim(0);
            int taskCount = 0;
            List<Task> tasks = new List<Task>();

            scheduler.ScheduleAsync(async () =>
            {
                await Task.Delay(200);
                 // Simulate that this event takes a really long time.
                taskCount++;
            })
            .EveryMinute()
            .PreventOverlapping("PreventOverlappingTest");

            var longRunningTask = scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:00 am"));

            await Task.Delay(1); // Make sure above starts.

            await Task.WhenAll(
                scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:01 am")),
                scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:02 am")),
                scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:03 am"))
            );

            await longRunningTask;
 
            // We should have only ever executed the scheduled task once.
            Assert.Equal(1, taskCount);
        }

       // [Fact]
        public async Task OverlapNotPrevented()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
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