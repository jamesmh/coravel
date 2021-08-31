using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel;
using Coravel.Events;
using Coravel.Events.Interfaces;
using Coravel.Queuing;
using Coravel.Queuing.Broadcast;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Events.EventsAndListeners;
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

         [Fact]
        public async Task TestQueuedTaskStartedAndCompletedEvents()
        {
            var services = new ServiceCollection();
            services.AddEvents();
            services.AddTransient<TaskStartedListener>();
            services.AddTransient<TaskCompletedListener>();
            var provider = services.BuildServiceProvider();

            var dispatcher = provider.GetRequiredService<IDispatcher>() as Dispatcher;
            dispatcher.Register<QueueTaskStarted>().Subscribe<TaskStartedListener>();
            dispatcher.Register<QueueTaskCompleted>().Subscribe<TaskCompletedListener>();

            var queue = new Queue(null, dispatcher);

            // Note: Depending on where the tests are run from (local pc vs. GitHub actions runner for example),
            // the delay here may not be enough for the test to pass. GitHub actions runners were failing this
            // test often so I gradually increased the delay here until I have a value that seems like
            // it doesn't fail on the runners.
            var id1 = queue.QueueAsyncTask(() => Task.Delay(600));
            var id2 = queue.QueueAsyncTask(() => Task.Delay(600));
            var id3 = queue.QueueAsyncTask(() => Task.CompletedTask);

            Assert.Empty(TaskStartedListener.StartedJobs);
            Assert.Empty(TaskCompletedListener.CompletedJobs);

            var consumingTask = queue.ConsumeQueueAsync();
            await Task.Delay(1);

            Assert.Single(TaskCompletedListener.CompletedJobs);
            Assert.Equal(3, TaskStartedListener.StartedJobs.Count());
            Assert.Equal(id3, TaskCompletedListener.CompletedJobs.First());

            await consumingTask;

            Assert.Equal(3, TaskStartedListener.StartedJobs.Count());
            Assert.Equal(3, TaskCompletedListener.CompletedJobs.Count());
        }

        private class TaskStartedListener : IListener<QueueTaskStarted>
        {
            public static List<Guid> StartedJobs { get; set; } = new List<Guid>();

            public Task HandleAsync(QueueTaskStarted broadcasted)
            {
                lock(StartedJobs)
                {
                    StartedJobs.Add(broadcasted.Guid);
                }
                return Task.CompletedTask;
            }
        }

        private class TaskCompletedListener : IListener<QueueTaskCompleted>
        {
            public static List<Guid> CompletedJobs { get; set; } = new List<Guid>();

            public Task HandleAsync(QueueTaskCompleted broadcasted)
            {
                lock(CompletedJobs)
                {
                    CompletedJobs.Add(broadcasted.Guid);
                }
                return Task.CompletedTask;
            }
        }
    }
}