using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Tests.Scheduling.Helpers.SchedulingTestHelpers;

namespace Tests.Scheduling
{
    [TestClass]
    public class SchedulerOnErrorTests
    {
        [TestMethod]
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

            Assert.IsTrue(errorHandledCount == 2);
            Assert.IsTrue(successfulTaskCount == 4);
        }

         [TestMethod]
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

            Assert.IsTrue(successfulTaskCount == 1);
        }


        // add test for tasks with errors when theres no handler
    }
}