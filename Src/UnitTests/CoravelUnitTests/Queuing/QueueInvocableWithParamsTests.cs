using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Queuing;
using CoravelUnitTests.Scheduling.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CoravelUnitTests.Queuing
{
	public class QueueInvocableWithParamsTests
	{
		[Fact]
		public async Task TestQueueInvocableWithComplexParams()
		{
			int testNumber = 0;
			string testString = "";

			var parameters = new TestParams
			{
				Name = "This is valid",
				Number = 999
			};

			var services = new ServiceCollection();
			services.AddScoped<Action<string, int>>(p => (str, num) =>
			{
				testNumber = num;
				testString = str;
			});
			services.AddScoped<TestInvocable>();
			var provider = services.BuildServiceProvider();

			var queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());
			queue.QueueInvocableWithPayload<TestInvocable, TestParams>(parameters);
			await queue.ConsumeQueueAsync();

			Assert.Equal(999, testNumber);
			Assert.Equal("This is valid", testString);
		}

		[Fact]
		public async Task TestQueueInvocableWithPrimitiveParams()
		{
			string testString = "";

			var parameters = "This is valid";

			var services = new ServiceCollection();
			services.AddScoped<Action<string>>(p => str => testString = str);
			services.AddScoped<TestInvocableWithStringParam>();
			var provider = services.BuildServiceProvider();

			var queue = new Queue(provider.GetRequiredService<IServiceScopeFactory>(), new DispatcherStub());
			queue.QueueInvocableWithPayload<TestInvocableWithStringParam, string>(parameters);
			await queue.ConsumeQueueAsync();

			Assert.Equal("This is valid", testString);
		}
	}

	public class TestParams
	{
		public string Name { get; set; }
		public int Number { get; set; }
	}

	public class TestInvocable : IInvocable, IInvocableWithPayload<TestParams>
	{
		public TestParams Payload { get; set; }
		private Action<string, int> _func;

		public TestInvocable(Action<string, int> func) => this._func = func;

		public Task Invoke()
		{
			this._func(this.Payload.Name, this.Payload.Number);
			return Task.CompletedTask;
		}
	}

	public class TestInvocableWithStringParam : IInvocable, IInvocableWithPayload<string>
	{
		public string Payload { get; set; }

		private Action<string> _func;

		public TestInvocableWithStringParam(Action<string> func) => this._func = func;

		public Task Invoke()
		{
			this._func(this.Payload);
			return Task.CompletedTask;
		}
	}
}