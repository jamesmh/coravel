using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using CoravelUnitTests.Queuing;
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
            queue.QueueAsyncTask(async () =>
            {
                await Task.Delay(200);
                throw new Exception("Test exception");
            });
            queue.QueueAsyncTask(() => Task.Delay(200));

            Assert.Equal(3, queue.GetMetrics().WaitingCount());

            var consumingTask = queue.ConsumeQueueAsync();
            await Task.Delay(2);

            var metrics = queue.GetMetrics();
            Assert.Equal(0, metrics.WaitingCount());
            Assert.Equal(3, metrics.RunningCount());

            await consumingTask;

            metrics = queue.GetMetrics();
            Assert.Equal(0, metrics.RunningCount());
        }

        [Fact]
        public async Task TestAllGuidMethods()
        {
            Queue queue = new Queue(null, new DispatcherStub());

            var guid1 = queue.QueueTask(() => Task.Delay(1));
            var guid2 = queue.QueueAsyncTask(() => Task.Delay(200));
            var (guid3, _) = queue.QueueCancellableInvocable<DummyInvocable>();
            var guid4 = queue.QueueInvocable<DummyInvocable>();
            var guid5 = queue.QueueInvocableWithPayload<DummyInvocable, TestParams>(new TestParams());

            Assert.False(string.IsNullOrWhiteSpace(guid1.ToString()));
            Assert.False(string.IsNullOrWhiteSpace(guid2.ToString()));
            Assert.False(string.IsNullOrWhiteSpace(guid3.ToString()));
            Assert.False(string.IsNullOrWhiteSpace(guid5.ToString()));
            Assert.False(string.IsNullOrWhiteSpace(guid5.ToString()));

            await queue.ConsumeQueueAsync();
        }  

        public class TestParams
        {
        }

        public class DummyInvocable : IInvocable, ICancellableTask, IInvocableWithPayload<TestParams>
        {
            public DummyInvocable() {}

            public TestParams Payload { get; set; }

            CancellationToken ICancellableTask.Token { get; set; }

           public Task Invoke()
            {
                return Task.CompletedTask;
            }
        }   
    }
}