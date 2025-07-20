using System.Threading.Tasks;
using CoravelUnitTests.Scheduling.Stubs;
using Coravel.Invocable;
using Coravel.Invocable.Interfaces;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Mutex;
using Coravel.Queuing;
using Coravel;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Threading;
using System;

namespace CoravelUnitTests.Scheduling.GlobalPreventOverlappingTests
{
    public class GlobalPreventOverlappingTests
    {
        // Test invocable for global prevent overlapping functionality
        public class TestGlobalPreventOverlapInvocable : IInvocable
        {
            public static int ExecutionCount = 0;
            public static SemaphoreSlim ExecutionSemaphore = new SemaphoreSlim(0);

            public async Task Invoke()
            {
                Interlocked.Increment(ref ExecutionCount);
                ExecutionSemaphore.Release();
                await Task.Delay(200); // Simulate long running task
            }
        }

        // Test invocable for global prevent overlapping functionality
        public class TestGlobalPreventOverlapInvocable2 : IInvocable
        {
            public static int ExecutionCount = 0;

            public async Task Invoke()
            {
                Interlocked.Increment(ref ExecutionCount);
                await Task.Delay(100); // Simulate task
            }
        }

        [Fact]
        public async Task GlobalPreventOverlapping_WithScheduler_ShouldPreventOverlappingExecution()
        {
            // Reset counters
            TestGlobalPreventOverlapInvocable.ExecutionCount = 0;
            TestGlobalPreventOverlapInvocable.ExecutionSemaphore = new SemaphoreSlim(0);

            var services = new ServiceCollection();
            services.AddScoped<TestGlobalPreventOverlapInvocable>();
            var provider = services.BuildServiceProvider();

            var globalConfig = new CoravelGlobalConfiguration();
            globalConfig.RegisterPreventOverlapping<TestGlobalPreventOverlapInvocable>("global-prevent-overlap-test");

            var scheduler = new Scheduler(new InMemoryMutex(), provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub(), globalConfig);

            // Schedule the same invocable multiple times
            scheduler.Schedule<TestGlobalPreventOverlapInvocable>().EveryMinute();

            // Run the scheduler multiple times quickly
            var task1 = scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:00 am"));
            await Task.Delay(1); // Make sure above starts first
            
            var task2 = scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:01 am"));
            var task3 = scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:02 am"));

            // Wait for at least one execution to start
            await TestGlobalPreventOverlapInvocable.ExecutionSemaphore.WaitAsync(TimeSpan.FromSeconds(5));

            // Wait for all scheduler runs to complete
            await Task.WhenAll(task1, task2, task3);

            // Only one execution should have happened due to global prevent overlapping
            Assert.Equal(1, TestGlobalPreventOverlapInvocable.ExecutionCount);
        }

        [Fact]
        public async Task GlobalPreventOverlapping_WithQueue_ShouldPreventOverlappingExecution()
        {
            // Reset counters
            TestGlobalPreventOverlapInvocable2.ExecutionCount = 0;

            var services = new ServiceCollection();
            services.AddScoped<TestGlobalPreventOverlapInvocable2>();
            var provider = services.BuildServiceProvider();

            var globalConfig = new CoravelGlobalConfiguration();
            globalConfig.RegisterPreventOverlapping<TestGlobalPreventOverlapInvocable2>("global-prevent-overlap-queue-test");

            var queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub(), new InMemoryMutex(), globalConfig);

            // Queue the same invocable multiple times
            queue.QueueInvocable<TestGlobalPreventOverlapInvocable2>();
            queue.QueueInvocable<TestGlobalPreventOverlapInvocable2>();
            queue.QueueInvocable<TestGlobalPreventOverlapInvocable2>();

            // Consume the queue
            await queue.ConsumeQueueAsync();

            // Only one execution should have happened due to global prevent overlapping
            // Note: In the queue implementation, overlapping tasks are consumed but not executed
            Assert.Equal(1, TestGlobalPreventOverlapInvocable2.ExecutionCount);
        }

        [Fact]
        public void GlobalPreventOverlapping_WithServiceProvider_ShouldConfigureCorrectly()
        {
            var services = new ServiceCollection();
            services.AddSingleton<TestGlobalPreventOverlapInvocable>();
            services.AddSingleton<ICoravelGlobalConfiguration, CoravelGlobalConfiguration>();

            var provider = services.BuildServiceProvider();

            // Configure global prevent overlapping using the extension method
            provider.PreventOverlapping<TestGlobalPreventOverlapInvocable>("service-provider-test");

            // Verify the configuration was registered
            var globalConfig = provider.GetRequiredService<ICoravelGlobalConfiguration>();
            var hasConfiguration = globalConfig.TryGetPreventOverlapping<TestGlobalPreventOverlapInvocable>(out string identifier);

            Assert.True(hasConfiguration);
            Assert.Equal("service-provider-test", identifier);
        }

        [Fact]
        public void GlobalPreventOverlapping_WithoutConfiguration_ShouldReturnFalse()
        {
            var globalConfig = new CoravelGlobalConfiguration();

            // Try to get configuration for an invocable that hasn't been configured
            var hasConfiguration = globalConfig.TryGetPreventOverlapping<TestGlobalPreventOverlapInvocable>(out string identifier);

            Assert.False(hasConfiguration);
            Assert.Null(identifier);
        }

        [Fact]
        public void GlobalPreventOverlapping_WithEmptyIdentifier_ShouldThrowException()
        {
            var globalConfig = new CoravelGlobalConfiguration();

            // Registering with empty identifier should throw exception
            Assert.Throws<ArgumentException>(() => 
                globalConfig.RegisterPreventOverlapping<TestGlobalPreventOverlapInvocable>(""));
        }

        [Fact]
        public async Task GlobalPreventOverlapping_SchedulerWithExistingPreventOverlap_ShouldUseEventLevel()
        {
            // Reset counters
            TestGlobalPreventOverlapInvocable.ExecutionCount = 0;
            TestGlobalPreventOverlapInvocable.ExecutionSemaphore = new SemaphoreSlim(0);

            var services = new ServiceCollection();
            services.AddScoped<TestGlobalPreventOverlapInvocable>();
            var provider = services.BuildServiceProvider();

            var globalConfig = new CoravelGlobalConfiguration();
            globalConfig.RegisterPreventOverlapping<TestGlobalPreventOverlapInvocable>("global-identifier");

            var scheduler = new Scheduler(new InMemoryMutex(), provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub(), globalConfig);

            // Schedule with event-level prevent overlapping (should take precedence over global)
            scheduler.Schedule<TestGlobalPreventOverlapInvocable>()
                     .EveryMinute()
                     .PreventOverlapping("event-level-identifier");

            // Run the scheduler multiple times quickly
            var task1 = scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:00 am"));
            await Task.Delay(1); // Make sure above starts first
            
            var task2 = scheduler.RunAtAsync(DateTime.Parse("2018/01/01 00:01 am"));

            // Wait for at least one execution to start
            await TestGlobalPreventOverlapInvocable.ExecutionSemaphore.WaitAsync(TimeSpan.FromSeconds(5));

            // Wait for all scheduler runs to complete
            await Task.WhenAll(task1, task2);

            // Only one execution should have happened (event-level prevent overlapping took precedence)
            Assert.Equal(1, TestGlobalPreventOverlapInvocable.ExecutionCount);
        }
    }
}