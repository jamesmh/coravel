using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Queuing;
using Coravel.Queuing.Interfaces;
using CoravelUnitTests.Scheduling.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoravelUnitTests.Queuing
{
	public class CancellableInvocableForQueueTests
	{
		[Fact]
		public async Task CanCancelInvocable()
		{
			var services = new ServiceCollection();
			services.AddTransient<TestCancellableInvocable>();
			var provider = services.BuildServiceProvider();

			Queue queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());

			var (_, token1) = queue.QueueCancellableInvocable<TestCancellableInvocable>();
			var (_, token2) = queue.QueueCancellableInvocable<TestCancellableInvocable>();
			var (_, token3) = queue.QueueCancellableInvocable<TestCancellableInvocable>();

			token1.Cancel();
			token3.Cancel();

			TestCancellableInvocable.TokensCancelled = 0;
			await queue.ConsumeQueueAsync();

			Assert.Equal(2, TestCancellableInvocable.TokensCancelled);
		}

		[Fact]
		public async Task CanCancelInvocablesForShutdown()
		{
			var services = new ServiceCollection();
			services.AddTransient<TestCancellableInvocable>();
			var provider = services.BuildServiceProvider();

			Queue queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());

			var token1 = queue.QueueCancellableInvocable<TestCancellableInvocable>();
			var token2 = queue.QueueCancellableInvocable<TestCancellableInvocable>();
			var token3 = queue.QueueCancellableInvocable<TestCancellableInvocable>();

			TestCancellableInvocable.TokensCancelled = 0;
			await queue.ConsumeQueueOnShutdown();

			Assert.Equal(3, TestCancellableInvocable.TokensCancelled);
		}

		[Fact]
		public async Task CanCancelInvocableType()
		{
			var services = new ServiceCollection();

			var invocableType = typeof(IInvocable);
			var cancellableTaskType = typeof(ICancellableTask);

			var invocableAndCancellable = GetType().Assembly.GetTypes()
				.Where(x => invocableType.IsAssignableFrom(x) && cancellableTaskType.IsAssignableFrom(x))
				.ToList();

			foreach (var implementingType in invocableAndCancellable)
			{
				services.AddTransient(invocableType, implementingType);
			}

			var provider = services.BuildServiceProvider();

			Queue queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());

			var invocables = provider.GetRequiredService<IEnumerable<IInvocable>>();

			foreach (var implementingType in invocables)
			{
				var (_, token) = queue.QueueCancellableInvocable(implementingType);

				token.Cancel();
			}

			TestCancellableInvocable.TokensCancelled = 0;

			await queue.ConsumeQueueAsync();

			Assert.Equal(1, TestCancellableInvocable.TokensCancelled);
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

	public class TestCancellableInvocable : IInvocable, ICancellableTask
	{
		/// <summary>
		/// Static fields keeps track of all cancelled tokens count.
		/// </summary>
		public static int TokensCancelled = 0;

		public TestCancellableInvocable() { }

		public CancellationToken Token { get; set; }

		public Task Invoke()
		{
			if (this.Token.IsCancellationRequested)
			{
				Interlocked.Increment(ref TokensCancelled);
			}

			return Task.CompletedTask;
		}
	}
}