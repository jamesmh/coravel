using System;
using System.Threading.Tasks;
using Coravel.Queuing;
using UnitTests.Scheduling.Stubs;
using Xunit;

namespace UnitTests.Queuing
{
    public class QueueMetricTests
    {
        [Fact]
        public async Task TestQueueHasCorrectNumberOfJobsWaiting()
        {
            Queue queue = new Queue(null, new DispatcherStub());

            queue.QueueTask(() => Console.Write("test-job-1"));
            queue.QueueTask(() => Console.Write("test-job-2"));
            queue.QueueTask(() => Console.Write("test-job-3"));

            Assert.Equal(3, queue.GetMetrics().WaitingCount());
            await queue.ConsumeQueueAsync();
            Assert.Equal(0, queue.GetMetrics().WaitingCount());
        }

        [Fact]
        public async Task TestQueueHasCorrectNumberOfJobsRunning()
        {
            Queue queue = new Queue(null, new DispatcherStub());

            queue.QueueAsyncTask(() => Task.Delay(200));
            queue.QueueAsyncTask(() => Task.Delay(200));
            queue.QueueAsyncTask(() => Task.Delay(200));

            Assert.Equal(3, queue.GetMetrics().WaitingCount());

            var consumingTask = queue.ConsumeQueueAsync();
            await Task.Delay(2);

            var metrics = queue.GetMetrics();
            Assert.Equal(0, metrics.WaitingCount());
            Assert.Equal(3, metrics.RunningCount());

            await consumingTask;
        }       
    }
}