using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Scheduling.Stubs;
using Xunit;

namespace CoravelUnitTests.Scheduling.Invocable
{
    public class CancellableInvocableTests
    {

        [Fact]
        public async Task TestInvocableCanBeCancelled()
        {
            bool invocableCancelled = false;

            var services = new ServiceCollection();
            services.AddScoped<Action>(p => () => invocableCancelled = true);
            services.AddScoped<CancellableInvocable>();
            var provider = services.BuildServiceProvider();

            var scheduler = new Scheduler(new InMemoryMutex(), provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());
            
            scheduler.Schedule<CancellableInvocable>().EveryMinute();

            var schedulerTask = scheduler.RunAtAsync(new DateTime(2019, 1, 1));

            scheduler.CancelAllCancellableTasks();

            await schedulerTask;

            Assert.True(invocableCancelled);
        }


        private class CancellableInvocable : IInvocable, ICancellableInvocable
        {
            public CancellationToken CancellationToken { get; set; }

            private readonly Action _func;

            public CancellableInvocable(Action func)
            {
                this._func = func;
            }

            public async Task Invoke()
            {
                await Task.Delay(500, CancellationToken)
                    .ContinueWith(task => this._func(), TaskContinuationOptions.OnlyOnCanceled);
            }
        }
    }
}
