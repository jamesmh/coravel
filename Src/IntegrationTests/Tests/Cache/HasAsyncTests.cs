using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Cache
{
    public class HasAsyncTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
    {
        private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

        public HasAsyncTests(WebApplicationFactory<TestMvcApp.Startup> factory)
        {
            this._factory = factory;
        }

        [Fact]
        public async Task Has_Async_Cache_Working()
        {
            var client = _factory.CreateClient();

            var response = await client.GetStringAsync("/Cache/HasAsync");
            var responseTwo = await client.GetStringAsync("/Cache/Remember");
            var responseThree = await client.GetStringAsync("/Cache/HasAsync");

            Assert.Equal(false.ToString(), response);
            Assert.Equal(Constants.WasCachedResult, responseTwo);
            Assert.Equal(true.ToString(), responseThree);
        }
    }
}
