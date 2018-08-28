using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace UnitTests.Scheduling.Invocable
{
    public class InvocableTests
    {
        [Fact]
        public async Task TestScheduledInvocableRuns()
        {
            bool invocableRan = false;
             var services = new ServiceCollection();
            services.AddScoped<Action>(p => () => invocableRan = true);
            services.AddScoped<TestInvocable>();
            var provider = services.BuildServiceProvider();

            var scheduler = new Scheduler(new InMemoryMutex(), provider.GetRequiredService<IServiceScopeFactory>());
            scheduler.Schedule<TestInvocable>().EveryMinute();

            await scheduler.RunSchedulerAsync();

            Assert.True(invocableRan);
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