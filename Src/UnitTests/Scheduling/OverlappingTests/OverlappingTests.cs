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
        [Fact]
        public async Task LongRunningEventPreventOverlap()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub());
            var semaphore = new SemaphoreSlim(0);
            bool keepRunning = true;
            int taskCount = 0;
            List<Task> tasks = new List<Task>();

            scheduler.ScheduleAsync(async () =>
            {
                while (keepRunning)
                {
                    await Task.Delay(1);
                } // Simulate that this event takes a really long time.
                taskCount++;
            })
            .EveryMinute()
            .PreventOverlapping("PreventOverlappingTest");

            var longRunningTask = Task.Run(async () => await scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:00 am")));

            await Task.Yield(); // Let the task above run.

            // Attempt to run the scheduler mulitple times.
            // We use Task.Run otherwise the code will block if this test fails...
            tasks.AddRange(new Task[] {        
                Task.Run(async () => await scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:01 am"))),
                Task.Run(async () => await scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:02 am"))),
                Task.Run(async () => await scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:03 am"))),
                Task.Run(async () => await scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:04 am"))),
                Task.Run(async () => await scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:05 am"))),
                Task.Run(async () => await scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:06 am")))
            });

            await Task.WhenAll(tasks); // None of these should have executed.

            keepRunning = false;
            await longRunningTask;
 
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