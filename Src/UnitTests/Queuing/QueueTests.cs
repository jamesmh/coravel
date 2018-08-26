using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using Coravel.Scheduling.Schedule;
using Microsoft.Extensions.DependencyInjection;
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

            Queue queue = new Queue(null);

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

            Queue queue = new Queue(null);

            queue.QueueTask(() => successfulTasks++);
            queue.QueueTask(() => throw new Exception());
            queue.QueueTask(() => successfulTasks++);

            await queue.ConsumeQueueAsync();

            Assert.Equal(2, successfulTasks);
        }

        [Fact]
        public async Task TestQueueInvocable()
        {
            int successfulTasks = 0;
            var services = new ServiceCollection();
            services.AddScoped<Action>(p => () => successfulTasks++);
            services.AddScoped<TestInvocable>();
            var provider = services.BuildServiceProvider();

            var queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>());
            queue.QueueInvocable<TestInvocable>();
            await queue.ConsumeQueueAsync();
            await queue.ConsumeQueueAsync();

            Assert.Equal(1, successfulTasks);                    
        }

        private class TestInvocable : IInvocable
        {
            private Action _func;

            public TestInvocable(Action func) => this._func = func;
            public Task Invoke()
            {
                this._func();
                return Task.CompletedTask;
            }
        }
    }
}