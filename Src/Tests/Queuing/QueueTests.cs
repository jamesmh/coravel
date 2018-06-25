using System;
using System.Threading.Tasks;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Queuing
{
    [TestClass]
    public class QueueTests
    {
        [TestMethod]
        public async Task TestQueueRunsProperly()
        {
            int errorsHandled = 0;
            int successfulTasks = 0;

            Queue queue = new Queue();

            queue.OnError(ex => errorsHandled++);

            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => throw new Exception());
            queue.QueueTask(() => successfulTasks++);

            await queue.ConsumeQueueAsync();

            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => throw new Exception());

            await queue.ConsumeQueueAsync(); // Consume the two above.

            // These should not get executed.
            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => throw new Exception());

            Assert.IsTrue(errorsHandled == 2);
            Assert.IsTrue(successfulTasks == 4);
        }

        [TestMethod]
        public async Task TestQueueSlientErrors()
        {
            int successfulTasks = 0;

            Queue queue = new Queue();

            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => throw new Exception());
            queue.QueueTask(() => successfulTasks++);

            await queue.ConsumeQueueAsync();

            Assert.AreEqual(2, successfulTasks);
        }
    }
}