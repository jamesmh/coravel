using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;
using static UnitTests.Scheduling.Helpers.SchedulingTestHelpers;

namespace UnitTests.Scheduling.IntervalTests
{
    public class SchedulerOnErrorTests
    {
        [Fact]
        public async Task TestSchedulerHandlesErrors()
        {
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());
            int errorHandledCount = 0;
            int successfulTaskCount = 0;

            void DummyTask()
            {
                successfulTaskCount++;
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

            await scheduler.RunAtAsync(new DateTime(2019, 1, 1)); // All tasks will run.

            Assert.True(errorHandledCount == 2);
            Assert.True(successfulTaskCount == 4);
        }

        [Fact]
        public async Task TestSchedulerSkipsErrors()
        {
            int successfulTaskCount = 0;
            var scheduler = new Scheduler(new InMemoryMutex(), new ServiceScopeFactoryStub(), new DispatcherStub());

            void DummyTask()
            {
                successfulTaskCount++;
            };

            void ThrowsErrorTask()
            {
                throw new Exception("dummy");
            }

            scheduler.Schedule(ThrowsErrorTask).EveryMinute();
            scheduler.Schedule(DummyTask).EveryMinute();

            await scheduler.RunAtAsync(new DateTime(2019, 1, 1));

            Assert.True(successfulTaskCount == 1);
        }
    }
}