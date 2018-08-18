using System;
using System.Threading.Tasks;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule;
using Xunit;

namespace UnitTests.Queuing
{
    public class QueueTests
    {
        [Fact]
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

            Assert.True(errorsHandled == 2);
            Assert.True(successfulTasks == 4);
        }

        [Fact]
        public async Task TestQueueSlientErrors()
        {
            int successfulTasks = 0;

            Queue queue = new Queue();

            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => throw new Exception());
            queue.QueueTask(() => successfulTasks++);

            await queue.ConsumeQueueAsync();

            Assert.Equal(2, successfulTasks);
        }
    }
}