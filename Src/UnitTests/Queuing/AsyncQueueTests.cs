using System;
using System.Threading.Tasks;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule;
using UnitTests.Scheduling.Stubs;
using Xunit;

namespace UnitTests.Queuing
{
    public class AsyncQueueTests
    {
        [Fact]
        public async Task TestAsyncQueueRunsProperly()
        {
            int errorsHandled = 0;
            int successfulTasks = 0;

            Queue queue = new Queue(null, new DispatcherStub());

            queue.OnError(ex => errorsHandled++);

            queue.QueueTask(() => successfulTasks++);
            queue.QueueAsyncTask(async () =>
            {
                await Task.Delay(1);
                successfulTasks++;
            });
            queue.QueueTask(() => throw new Exception());
            queue.QueueTask(() => successfulTasks++);

            await queue.ConsumeQueueAsync();

            queue.QueueTask(() => successfulTasks++);
            queue.QueueAsyncTask(async () =>
            {
                await Task.Delay(1);
                throw new Exception();
            });

            await queue.ConsumeQueueAsync(); // Consume the two above.

            // These should not get executed.
            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => throw new Exception());

            Assert.True(errorsHandled == 2);
            Assert.True(successfulTasks == 4);
        }
    }
}