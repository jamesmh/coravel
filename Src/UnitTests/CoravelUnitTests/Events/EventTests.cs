using System;
using System.Threading.Tasks;
using Coravel;
using Coravel.Events;
using Coravel.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Events.EventsAndListeners;
using Xunit;

namespace UnitTests.Events
{
    public class EventTests
    {
        [Fact]
        public async Task TestRegisterOneListener()
        {
            int listenersExecutedCount = 0;

            var services = new ServiceCollection();
            services.AddTransient<Action>(p => () => listenersExecutedCount++); // This is injected into the listeners via DI
            services.AddEvents();
            services.AddTransient<TestListener1ForEvent1>();
            services.AddTransient<TestListener2ForEvent1>();
            services.AddTransient<TestListenerForEvent2>();
            var provider = services.BuildServiceProvider();

            var dispatcher = provider.GetRequiredService<IDispatcher>() as Dispatcher;

            dispatcher.Register<TestEvent1>()
                .Subscribe<TestListener1ForEvent1>();

            await dispatcher.Broadcast(new TestEvent1());

            Assert.Equal(1, listenersExecutedCount);
        }

        [Fact]
        public async Task TestRegisterTwoListenerWithDifferentRegisterCall()
        {
            int listenersExecutedCount = 0;

            var services = new ServiceCollection();
            services.AddTransient<Action>(p => () => listenersExecutedCount++); // This is injected into the listeners via DI
            services.AddEvents();
            services.AddTransient<TestListener1ForEvent1>();
            services.AddTransient<TestListener2ForEvent1>();
            services.AddTransient<TestListenerForEvent2>();
            var provider = services.BuildServiceProvider();

            var dispatcher = provider.GetRequiredService<IDispatcher>() as Dispatcher;

            dispatcher.Register<TestEvent1>()
                .Subscribe<TestListener1ForEvent1>();

            dispatcher.Register<TestEvent1>()
                .Subscribe<TestListener2ForEvent1>();

            await dispatcher.Broadcast(new TestEvent1());

            Assert.Equal(2, listenersExecutedCount);
        }

        [Fact]
        public async Task TestRegisterAllListeners()
        {
            int listenersExecutedCount = 0;

            var services = new ServiceCollection();
            services.AddTransient<Action>(p => () => listenersExecutedCount++);
            services.AddEvents();
            services.AddTransient<TestListener1ForEvent1>();
            services.AddTransient<TestListener2ForEvent1>();
            services.AddTransient<TestListenerForEvent2>();
            var provider = services.BuildServiceProvider();

            var dispatcher = provider.GetRequiredService<IDispatcher>() as Dispatcher;

            dispatcher.Register<TestEvent1>()
                .Subscribe<TestListener1ForEvent1>()
                .Subscribe<TestListener2ForEvent1>();

            dispatcher.Register<TestEvent2>()
                .Subscribe<TestListenerForEvent2>();

            await dispatcher.Broadcast(new TestEvent1());
            Assert.Equal(2, listenersExecutedCount);

            await dispatcher.Broadcast(new TestEvent2());
            Assert.Equal(3, listenersExecutedCount);
        }

        [Fact]
        public async Task TestDuplicateSubscriptionsJustBypasses()
        {
            int listenersExecutedCount = 0;

            var services = new ServiceCollection();
            services.AddTransient<Action>(p => () => listenersExecutedCount++);
            services.AddEvents();
            services.AddTransient<TestListener1ForEvent1>();
            services.AddTransient<TestListener2ForEvent1>();
            var provider = services.BuildServiceProvider();

            var dispatcher = provider.GetRequiredService<IDispatcher>() as Dispatcher;

            // We are testing this one.
            dispatcher.Register<TestEvent1>()
                .Subscribe<TestListener1ForEvent1>()
                .Subscribe<TestListener2ForEvent1>()
                .Subscribe<TestListener1ForEvent1>()
                .Subscribe<TestListener2ForEvent1>();

            await dispatcher.Broadcast(new TestEvent1());

            Assert.Equal(2, listenersExecutedCount);
        }

        [Fact]
        public async Task TestEventListenerThatDispatchesEvent1And2()
        {
            int listenersExecutedCount = 0;

            var services = new ServiceCollection();
            services.AddTransient<Action>(p => () => listenersExecutedCount++);
            services.AddEvents();
            services.AddTransient<TestListener1ForEvent1>();
            services.AddTransient<TestListener2ForEvent1>();
            services.AddTransient<TestListenerForEvent2>();
            services.AddTransient<TestListenerThatFiresEvent1And2>();
            var provider = services.BuildServiceProvider();

            var dispatcher = provider.GetRequiredService<IDispatcher>() as Dispatcher;

            dispatcher.Register<TestEvent1>()
                .Subscribe<TestListener1ForEvent1>()
                .Subscribe<TestListener2ForEvent1>();

            dispatcher.Register<TestEvent2>()
                .Subscribe<TestListenerForEvent2>();

            dispatcher.Register<TestEventWithDispatcher>()
                .Subscribe<TestListenerThatFiresEvent1And2>();

            await dispatcher.Broadcast(new TestEventWithDispatcher(dispatcher));
            Assert.Equal(3, listenersExecutedCount);
        }

        [Fact]
        public async Task TestRegisterSameTypeTwiceDoesntThrow()
        {
            int listenersExecutedCount = 0;

            var services = new ServiceCollection();
            services.AddTransient<Action>(p => () => listenersExecutedCount++);
            services.AddEvents();
            services.AddTransient<TestListener1ForEvent1>();
            var provider = services.BuildServiceProvider();

            var dispatcher = provider.GetRequiredService<IDispatcher>() as Dispatcher;

            dispatcher.Register<TestEvent1>()
                .Subscribe<TestListener1ForEvent1>();

            dispatcher.Register<TestEvent1>()
                .Subscribe<TestListener1ForEvent1>();

            await dispatcher.Broadcast(new TestEvent1());

            Assert.Equal(1, listenersExecutedCount);
        }
    }
}