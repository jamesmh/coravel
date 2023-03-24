using System;
using System.Threading.Tasks;
using Coravel;
using Coravel.Events;
using Coravel.Events.Interfaces;
using CoravelUnitTests.Events.EventsAndListeners;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoravelUnitTests.Events
{
    public class ListenerDisposableTests
    {
        [Fact]
        public async Task TestListenerWasDisposedAndDisposedAsync()
        {
            bool wasDisposed = false;
            bool wasDisposedAsync = false;

            var services = new ServiceCollection();
            services.AddTransient<Action>(p => () => wasDisposed = true);
            services.AddScoped<Func<Task>>(p => async () =>
            {
                wasDisposedAsync = true;
                await Task.CompletedTask;
            });
            services.AddEvents();
            services.AddTransient<DisposableListener>();
            services.AddTransient<AsyncDisposableListener>();
            var provider = services.BuildServiceProvider();

            var dispatcher = provider.GetRequiredService<IDispatcher>() as Dispatcher;

            // We are testing this one.
            dispatcher.Register<TestEvent1>()
                .Subscribe<DisposableListener>()
                .Subscribe<AsyncDisposableListener>();

            await dispatcher.Broadcast(new TestEvent1());

            Assert.True(wasDisposed);
            Assert.True(wasDisposedAsync);
        }

        private class DisposableListener : IListener<TestEvent1>, IDisposable
        {
            private Action _disposalFunc;

            public DisposableListener(Action disposalFunc)
            {
                this._disposalFunc = disposalFunc;
            }

            public void Dispose()
            {
                this._disposalFunc?.Invoke();
            }

            public async Task HandleAsync(TestEvent1 broadcasted)
            {
                await Task.CompletedTask;
            }
        }
        
        private class AsyncDisposableListener : IListener<TestEvent1>, IAsyncDisposable
        {
            private Func<Task> _disposalFunc;

            public AsyncDisposableListener( Func<Task> disposalFunc)
            {
                this._disposalFunc = disposalFunc;
            }

            public async ValueTask DisposeAsync()
            {
                await _disposalFunc?.Invoke();
            }

            public async Task HandleAsync(TestEvent1 broadcasted)
            {
                await Task.CompletedTask;
            }
        }
    }
}
