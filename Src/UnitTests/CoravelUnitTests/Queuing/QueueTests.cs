using System;
using System.Threading.Tasks;
using Coravel;
using Coravel.Events.Interfaces;
using Coravel.Invocable;
using Coravel.Queuing;
using Coravel.Queuing.Broadcast;
using CoravelUnitTests.Events.EventsAndListeners;
using CoravelUnitTests.Scheduling.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoravelUnitTests.Queuing;

public class QueueTests
{
    [Fact]
    public async Task TestQueueRunsProperly()
    {
        int errorsHandled = 0;
        int successfulTasks = 0;

        Queue queue = new(null, new DispatcherStub());

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

        Queue queue = new(null, new DispatcherStub());

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

        var queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());
        queue.QueueInvocable<TestInvocable>();
        await queue.ConsumeQueueAsync();
        await queue.ConsumeQueueAsync();

        Assert.Equal(1, successfulTasks);
    }

    [Fact]
    public async Task<int> DoesNotThrowOnNullDispatcher()
    {
        int successfulTasks = 0;

        Queue queue = new(null, null);
        queue.QueueTask(() => successfulTasks++);
        queue.QueueTask(() => successfulTasks++);
        queue.QueueTask(() => successfulTasks++);

        await queue.ConsumeQueueAsync();
        // Should not throw due to null Dispatcher

        return successfulTasks;
    }

    [Fact]
    public async Task<int> QueueDispatchesInternalEvents()
    {

        var services = new ServiceCollection();
        services.AddEvents();
        services.AddTransient<QueueConsumationStartedListener>();
        var provider = services.BuildServiceProvider();

        IEventRegistration registration = provider.ConfigureEvents();

        registration
            .Register<QueueConsumationStarted>()
            .Subscribe<QueueConsumationStartedListener>();


        int successfulTasks = 0;

        Queue queue = new(provider.GetService<IServiceScopeFactory>(), provider.GetService<IDispatcher>()!);
        queue.QueueTask(() => successfulTasks++);
        queue.QueueTask(() => successfulTasks++);
        queue.QueueTask(() => successfulTasks++);

        await queue.ConsumeQueueAsync();
        // Should not throw due to null Dispatcher


        Assert.True(QueueConsumationStartedListener.Ran);

        return successfulTasks; // Avoids "unused variable" warning ;)     
    }

    private class TestInvocable : IInvocable
    {
        private readonly Action _func;

        public TestInvocable(Action func) => _func = func;
        public Task Invoke()
        {
            _func();
            return Task.CompletedTask;
        }
    }
}