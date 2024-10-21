using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using CoravelUnitTests.Scheduling.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoravelUnitTests.Queuing
{
    public class CancellableInvocableWithPayloadForQueueTests
    {
        [Fact]
        public async Task CanCancelInvocable()
        {
            int testNumber = 0;
            string testString = "";

            var param1 = new TestParams
            {
                Name = "String1",
                Number = 99
            };
            var param2 = new TestParams
            {
                Name = "String2",
                Number = 2
            };
            var param3 = new TestParams
            {
                Name = "String3",
                Number = 5
            };

            var services = new ServiceCollection();
            services.AddScoped<Action<string, int>>(p => (str, num) =>
            {
                testNumber += num;
                testString += str;
            });
            services.AddTransient<TestCancellableInvocableWithPayload>();
            var provider = services.BuildServiceProvider();

            Queue queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());

            var (_, token1) = queue.QueueCancellableInvocableWithPayload<TestCancellableInvocableWithPayload, TestParams>(param1);
            var (_, token2) = queue.QueueCancellableInvocableWithPayload<TestCancellableInvocableWithPayload, TestParams>(param2);
            var (_, token3) = queue.QueueCancellableInvocableWithPayload<TestCancellableInvocableWithPayload, TestParams>(param3);

            token1.Cancel();
            token3.Cancel();

            TestCancellableInvocableWithPayload.TokensCancelled = 0;
            await queue.ConsumeQueueAsync();

            Assert.Equal(2, TestCancellableInvocableWithPayload.TokensCancelled);
            Assert.Equal(param2.Number, testNumber);
            Assert.Equal(param2.Name, testString);
        }

        [Fact]
        public async Task CanCancelInvocablesForShutdown()
        {
            var services = new ServiceCollection();
            services.AddScoped<Action<string, int>>(p => (str, num) => { });
            services.AddTransient<TestCancellableInvocableWithPayload>();
            var provider = services.BuildServiceProvider();

            Queue queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());

            var (_, token1) = queue.QueueCancellableInvocableWithPayload<TestCancellableInvocableWithPayload, TestParams>(null);
            var (_, token2) = queue.QueueCancellableInvocableWithPayload<TestCancellableInvocableWithPayload, TestParams>(null);
            var (_, token3) = queue.QueueCancellableInvocableWithPayload<TestCancellableInvocableWithPayload, TestParams>(null);

            TestCancellableInvocableWithPayload.TokensCancelled = 0;
            await queue.ConsumeQueueOnShutdown();

            Assert.Equal(3, TestCancellableInvocableWithPayload.TokensCancelled);            
        }

        private class TestCancellableInvocableWithPayload :
            IInvocable,
            ICancellableTask,
            IInvocableWithPayload<TestParams>
        {
            private Action<string, int> _func;
            /// <summary>
            /// Static fields keeps track of all cancelled tokens count.
            /// </summary>
            public static int TokensCancelled = 0;

            public TestCancellableInvocableWithPayload(Action<string, int> func) =>
                this._func = func;

            public CancellationToken Token { get; set; }
            public TestParams Payload { get; set; }

            public Task Invoke()
            {
                if (this.Token.IsCancellationRequested)
                {
                    Interlocked.Increment(ref TokensCancelled);
                    return Task.CompletedTask;
                }

                this._func(this.Payload.Name, this.Payload.Number);
                return Task.CompletedTask;
            }
        }
    }
}