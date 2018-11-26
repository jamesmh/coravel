using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Cache
{
    public class RememberAsyncTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
    {
        private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

        public RememberAsyncTests(WebApplicationFactory<TestMvcApp.Startup> factory)
        {
            this._factory = factory;
        }

        [Fact]
        public async Task Remember_Async_Cache_Working()
        {
            var client = _factory.CreateClient();

            var response = await client.GetStringAsync("/Cache/RememberAsync");
            var responseTwo = await client.GetStringAsync("/Cache/RememberAsync");
            var responseThree = await client.GetStringAsync("/Cache/RememberAsync");

            Assert.Equal(Constants.WasCachedResult, response);
            Assert.Equal(Constants.NotCachedResult, responseTwo);
            Assert.Equal(Constants.NotCachedResult, responseThree);
        }
    }
}
