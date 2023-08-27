using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Queuing;
using CoravelUnitTests.Scheduling.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoravelUnitTests.Queuing;

public class InvocableDisposableTests
{
    [Fact]
    public async Task TestInvocableWasDisposed()
    {
        bool wasDisposed = false;
        var services = new ServiceCollection();
        services.AddScoped<Action>(p => () => wasDisposed = true);
        services.AddScoped<DisposableInvocable>();
        var provider = services.BuildServiceProvider();

        var queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());
        queue.QueueInvocable<DisposableInvocable>();
        await queue.ConsumeQueueAsync();

        Assert.True(wasDisposed);
    }
    
    [Fact]
    public async Task TestInvocableWasDisposedAsync()
    {
        bool wasDisposed = false;
        var services = new ServiceCollection();
        services.AddScoped<Func<Task>>(p => async () =>
        {
            wasDisposed = true;
            await Task.CompletedTask;
        });
        services.AddScoped<AsyncDisposableInvocable>();
        var provider = services.BuildServiceProvider();

        var queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());
        queue.QueueInvocable<AsyncDisposableInvocable>();
        await queue.ConsumeQueueAsync();

        Assert.True(wasDisposed);
    }
    
    private class DisposableInvocable : IInvocable, IDisposable
    {
        private readonly Action _disposalFunc;

        public DisposableInvocable(Action disposalFunc) => _disposalFunc = disposalFunc;

        public Task Invoke()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _disposalFunc?.Invoke();
        }
    }
    
    private class AsyncDisposableInvocable : IInvocable, IAsyncDisposable
    {
        private readonly Func<Task> _disposalFunc;

        public AsyncDisposableInvocable(Func<Task> disposalFunc) => _disposalFunc = disposalFunc;

        public Task Invoke()
        {
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await _disposalFunc?.Invoke();
        }
    }
}
