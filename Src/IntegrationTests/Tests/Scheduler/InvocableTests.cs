using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Scheduler
{
    public class InvocableTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
    {
        private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

        public InvocableTests(WebApplicationFactory<TestMvcApp.Startup> factory)
        {
            this._factory = factory;
        }

        [Fact]
        public async Task InvocableWithDIWorking()
        {
            var result = await this._factory.CreateClient().GetAsync("/Invocable/RunInvocableScheduledTask");
            result = await this._factory.CreateClient().GetAsync("/Invocable/RunInvocableScheduledTask");
            result = await this._factory.CreateClient().GetAsync("/Invocable/RunInvocableScheduledTask");
            result = await this._factory.CreateClient().GetAsync("/Invocable/RunInvocableScheduledTask");
            result = await this._factory.CreateClient().GetAsync("/Invocable/RunInvocableScheduledTask");
        }
    }
}