using System;
using System.Threading.Tasks;
using Coravel;
using Coravel.Events;
using Coravel.Events.Interfaces;
using Coravel.Invocable;
using Coravel.Queuing;
using Coravel.Queuing.Broadcast;
using Coravel.Queuing.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Events.EventsAndListeners;
using UnitTests.Scheduling.Stubs;
using Xunit;

namespace UnitTests.Queuing
{
    public class QueueBroadcastTests
    {
        [Fact]
        public async Task TestQueueBroadcastsEvents()
        {
            // Setup events

            int listenersExecutedCount = 0;

            var services = new ServiceCollection();
            services.AddTransient<Action>(p => () => listenersExecutedCount++); // This is injected into the listeners via DI
            services.AddEvents();
            services.AddTransient<TestListener1ForEvent1>();
            services.AddTransient<TestListener2ForEvent1>();
            var provider = services.BuildServiceProvider();

            var dispatcher = provider.GetRequiredService<IDispatcher>() as Dispatcher;

            dispatcher.Register<TestEvent1>()
                .Subscribe<TestListener1ForEvent1>()
                .Subscribe<TestListener2ForEvent1>();

            // Setup Queue

            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            var queue = new Queue(scopeFactory, dispatcher);

            queue.QueueBroadcast(new TestEvent1());

            await queue.ConsumeQueueAsync();

            Assert.True(listenersExecutedCount == 2);
        }
    }
}