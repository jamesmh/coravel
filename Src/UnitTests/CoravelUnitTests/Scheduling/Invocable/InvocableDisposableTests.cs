using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using CoravelUnitTests.Scheduling.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoravelUnitTests.Scheduling.Invocable;

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

        var scheduler = new Scheduler(new InMemoryMutex(), provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());
        scheduler.Schedule<DisposableInvocable>().EveryMinute();

        await scheduler.RunAtAsync(new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc));

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

        var scheduler = new Scheduler(new InMemoryMutex(), provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());
        scheduler.Schedule<AsyncDisposableInvocable>().EveryMinute();

        await scheduler.RunAtAsync(new DateTime(2019, 1, 1, 0, 0, 0, DateTimeKind.Utc));

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
            await _disposalFunc!.Invoke();
        }
    }
}
