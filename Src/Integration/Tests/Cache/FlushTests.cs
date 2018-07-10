using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Cache
{
    public class FlushTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
    {
        private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

        public FlushTests(WebApplicationFactory<TestMvcApp.Startup> factory)
        {
            this._factory = factory;
        }

        [Fact]
        public async Task FlushCacheWorking()
        {
            var client = _factory.CreateClient();

            var rememberResponse = await client.GetStringAsync("/Cache/Remember");
            await client.GetStringAsync("/Cache/Flush");
            var rememberResponseTwo = await client.GetStringAsync("/Cache/Remember");

            Assert.Equal(Constants.WasCachedResult, rememberResponse);
            Assert.Equal(Constants.WasCachedResult, rememberResponseTwo);
        }
    }
}