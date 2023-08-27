using System.Threading;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using CoravelUnitTests.Scheduling.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoravelUnitTests.Queuing;

public class CancellableInvocableForQueueTests
{
    [Fact]
    public async Task CanCancelInvocable()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestCancellableInvocable>();
        var provider = services.BuildServiceProvider();

        Queue queue = new(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());

        var (_, token1) = queue.QueueCancellableInvocable<TestCancellableInvocable>();
        var (_, token2) = queue.QueueCancellableInvocable<TestCancellableInvocable>();
        var (_, token3) = queue.QueueCancellableInvocable<TestCancellableInvocable>();

        token1.Cancel();
        token2.Cancel();
        token3.Cancel();

        TestCancellableInvocable.TokensCancelled = 0;
        await queue.ConsumeQueueAsync();

        Assert.Equal(3, TestCancellableInvocable.TokensCancelled);
    }

    [Fact]
    public async Task CanCancelInvocablesForShutdown()
    {
        var services = new ServiceCollection();
        services.AddTransient<TestCancellableInvocable>();
        var provider = services.BuildServiceProvider();

        Queue queue = new(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());

        _ = queue.QueueCancellableInvocable<TestCancellableInvocable>();
        _ = queue.QueueCancellableInvocable<TestCancellableInvocable>();
        _ = queue.QueueCancellableInvocable<TestCancellableInvocable>();

        TestCancellableInvocable.TokensCancelled = 0;
        await queue.ConsumeQueueOnShutdown();

        Assert.Equal(3, TestCancellableInvocable.TokensCancelled);
    }
    
    private class TestCancellableInvocable : IInvocable, ICancellableTask
    {
        /// <summary>
        /// Static fields keeps track of all cancelled tokens count.
        /// </summary>
        public static int TokensCancelled = 0;

        public TestCancellableInvocable() { }

        public CancellationToken Token { get; set; }

        public Task Invoke()
        {
            if (Token.IsCancellationRequested)
            {
                Interlocked.Increment(ref TokensCancelled);
            }

            return Task.CompletedTask;
        }
    }
}