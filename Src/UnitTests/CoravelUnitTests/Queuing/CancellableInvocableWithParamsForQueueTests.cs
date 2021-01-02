using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Scheduling.Stubs;
using Xunit;

namespace UnitTests.Queuing
{
    public class CancellableInvocableWithParamsForQueueTests
    {
        [Fact]
        public async Task CanCancelInvocable()
        {
            var services = new ServiceCollection();
            services.AddTransient<TestCancellableInvocable>();
            var provider = services.BuildServiceProvider();

            Queue queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());

            var payload = "Test";
            var (_, token1) = queue.QueueCancellableInvocableWithPayload<TestCancellableInvocable, string>(payload);
            var (_, token2) = queue.QueueCancellableInvocableWithPayload<TestCancellableInvocable, string>(payload);
            var (_, token3) = queue.QueueCancellableInvocableWithPayload<TestCancellableInvocable, string>(payload);

            token1.Cancel();
            token3.Cancel();

            TestCancellableInvocable.TokensCancelled = 0;
            await queue.ConsumeQueueAsync();

            Assert.Equal(2, TestCancellableInvocable.TokensCancelled);            
        }

        [Fact]
        public async Task CanCancelInvocablesForShutdown()
        {
            var services = new ServiceCollection();
            services.AddTransient<TestCancellableInvocable>();
            var provider = services.BuildServiceProvider();

            Queue queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());

            var payload = "Test";
            var token1 = queue.QueueCancellableInvocableWithPayload<TestCancellableInvocable, string>(payload);
            var token2 = queue.QueueCancellableInvocableWithPayload<TestCancellableInvocable, string>(payload);
            var token3 = queue.QueueCancellableInvocableWithPayload<TestCancellableInvocable, string>(payload);

            TestCancellableInvocable.TokensCancelled = 0;
            await queue.ConsumeQueueOnShutdown();

            Assert.Equal(3, TestCancellableInvocable.TokensCancelled);            
        }

        [Fact]
        public async Task TestQueueCancellableInvocableWithPrimitiveParams()
        {
            string testString = "";

            var parameters = "This is valid";

            var services = new ServiceCollection();
            services.AddScoped<Action<string>>(p => str => testString = str);
            services.AddScoped<TestCancellableInvocableWithStringParam>();
            var provider = services.BuildServiceProvider();

            var queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());
            queue.QueueCancellableInvocableWithPayload<TestCancellableInvocableWithStringParam, string>(parameters);
            await queue.ConsumeQueueAsync();

            Assert.Equal("This is valid", testString);
        }

        [Fact]
        public async Task TestQueueCancellableInvocableWithComplexParams()
        {
            int testNumber = 0;
            string testString = "";

            var parameters = new TestParams 
            {
                Name = "This is valid",
                Number = 999
            };

            var services = new ServiceCollection();
            services.AddScoped<Action<string, int>>(p => (str, num) => 
            { 
                testNumber = num; 
                testString = str;
            });
            services.AddScoped<TestCancellableInvocableWithComplexParams>();
            var provider = services.BuildServiceProvider();

            var queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());
            queue.QueueCancellableInvocableWithPayload<TestCancellableInvocableWithComplexParams, TestParams>(parameters);
            await queue.ConsumeQueueAsync();

            Assert.Equal(999, testNumber);
            Assert.Equal("This is valid", testString);
        }

        public class TestParams
        {
            public string Name { get; set; }
            public int Number { get; set; }
        }

        public class TestCancellableInvocableWithComplexParams : IInvocable, IInvocableWithPayload<TestParams>, ICancellableTask
        {
            public TestParams Payload { get; set; }
            private Action<string, int> _func;
            public CancellationToken Token { get; set; }

            public TestCancellableInvocableWithComplexParams(Action<string, int> func) => this._func = func;

            public Task Invoke()
            {
                this._func(this.Payload.Name, this.Payload.Number);
                return Task.CompletedTask;
            }
        }

        private class TestCancellableInvocable : IInvocable, ICancellableTask, IInvocableWithPayload<string>
        {
            /// <summary>
            /// Static fields keeps track of all cancelled tokens count.
            /// </summary>
            public static int TokensCancelled = 0;

            public TestCancellableInvocable() {}

            public CancellationToken Token { get; set; }

            public string Payload { get; set; }

            public Task Invoke()
            {
                if(this.Token.IsCancellationRequested)
                {
                    Interlocked.Increment(ref TokensCancelled);
                }

                return Task.CompletedTask;
            }
        }

        public class TestCancellableInvocableWithStringParam : IInvocable, IInvocableWithPayload<string>, ICancellableTask
        {
            public string Payload { get; set; }

            private Action<string> _func;

            public CancellationToken Token { get; set; }

            public TestCancellableInvocableWithStringParam(Action<string> func) => this._func = func;

            public Task Invoke()
            {
                this._func(this.Payload);
                return Task.CompletedTask;
            }
        }
    }
}

