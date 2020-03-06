using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Queuing;
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

        [Fact]
        public async Task TryEnqueueWhileRunnningLongRunningAsync()
        {
            int successfulTasks = 0;
            var semaphor = new SemaphoreSlim(0);

            Queue queue = new Queue(null, new DispatcherStub());

            queue.QueueAsyncTask(async () =>
            {
                await Task.Delay(10);
                successfulTasks++;
                semaphor.Release();
            });

            var runningTask = queue.ConsumeQueueAsync();

            await Task.Delay(10); // Make sure the queue is consuming.

            // Try to enqueue new tasks while queue is currently consuming.
            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => successfulTasks++);

            // Wait until the queue is done processing.
            await runningTask;
            // Start processing any tasks that were enqeued while it was running previously.
            await queue.ConsumeQueueAsync();

            Assert.True(successfulTasks == 5);
        }
    }
}