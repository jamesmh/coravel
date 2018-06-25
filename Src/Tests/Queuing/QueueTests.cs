using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Queuing
{
    [TestClass]
    public class QueueTests
    {
        [TestMethod]
        public async Task TestQueueRunsProperly(){
            int errorsHandled = 0;
            int successfulTasks = 0;

            var scheduler = new Scheduler();
            scheduler.OnError(ex => errorsHandled++); 
            var queue = scheduler.UseQueue();                  

            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => throw new Exception());
            queue.QueueTask(() => successfulTasks++);

            await scheduler.RunSchedulerAsync(); // This will consume the queue.
            
            queue.QueueTask(() => successfulTasks ++);
            queue.QueueTask(() => throw new Exception());

            await scheduler.RunSchedulerAsync(); // Consume the two above.

            // These should not get executed.
            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => throw new Exception());

            Assert.IsTrue(errorsHandled == 2);
            Assert.IsTrue(successfulTasks == 4);
        }
    }
}