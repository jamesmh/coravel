using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling
{
    public class SchedulerOnErrorTests
    {
        [Fact]
        public async Task TestSchedulerHandlesErrors()
        {
            var scheduler = new Scheduler();
            int errorHandledCount = 0;
            int successfulTaskCount = 0;

            void DummyTask()
            {
                successfulTaskCount++;
                Console.Write("dummy");
            };

            void ThrowsErrorTask()
            {
                throw new Exception("dummy");
            }

            // This is the method we are testing.
            scheduler.OnError((e) => errorHandledCount++);

            scheduler.Schedule(DummyTask).EveryMinute(); // Should run.
            scheduler.Schedule(ThrowsErrorTask).EveryMinute(); // Should error.
            scheduler.Schedule(DummyTask).EveryMinute(); // Should Run.
            scheduler.Schedule(ThrowsErrorTask).EveryMinute(); // Should error.
            scheduler.Schedule(DummyTask).EveryMinute(); // Should run.
            scheduler.Schedule(DummyTask).EveryMinute(); // Should run.

            await scheduler.RunAtAsync(DateTime.UtcNow); // All tasks will run.

            Assert.True(errorHandledCount == 2);
            Assert.True(successfulTaskCount == 4);
        }

        [Fact]
        public async Task TestSchedulerSkipsErrors()
        {
            int successfulTaskCount = 0;
            var scheduler = new Scheduler();

            void DummyTask()
            {
                successfulTaskCount++;
                Console.Write("dummy");
            };

            void ThrowsErrorTask()
            {
                throw new Exception("dummy");
            }

            scheduler.Schedule(ThrowsErrorTask).EveryMinute();
            scheduler.Schedule(DummyTask).EveryMinute();

            await scheduler.RunAtAsync(DateTime.UtcNow);

            Assert.True(successfulTaskCount == 1);
        }
    }
}