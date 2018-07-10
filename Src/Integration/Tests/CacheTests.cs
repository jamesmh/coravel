using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests
{
    public class CacheTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
    {
        private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;
        private readonly string WasCachedResult = "Was Cached:True";
        private readonly string NotCachedResult = "Was Cached:False";

        public CacheTests(WebApplicationFactory<TestMvcApp.Startup> factory)
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
            await client.GetStringAsync("/Cache/Flush"); // Flush since other tests use the cache.

            Assert.Equal(WasCachedResult, rememberResponse);
            Assert.Equal(WasCachedResult, rememberResponseTwo);
        }

        [Fact]
        public async Task RememberCacheWorking()
        {
            var client = _factory.CreateClient();

            var response = await client.GetStringAsync("/Cache/Remember");
            var responseTwo = await client.GetStringAsync("/Cache/Remember");
            var responseThree = await client.GetStringAsync("/Cache/Remember");
            await client.GetStringAsync("/Cache/Flush"); // Flush since other tests use the cache.

            Assert.Equal(WasCachedResult, response);
            Assert.Equal(NotCachedResult, responseTwo);
            Assert.Equal(NotCachedResult, responseThree);
        }
    }
}
