using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Cache
{
    public class GetTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
    {
        private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

        public GetTests(WebApplicationFactory<TestMvcApp.Startup> factory)
        {
            this._factory = factory;
        }

        [Fact]
        public async Task Get_Cache_Working()
        {
            var client = _factory.CreateClient();

            var response = await client.GetStringAsync("/Cache/Get");

            Assert.Equal(Constants.WasCachedResult, response);
        }

        [Fact]
        public async Task Get_Cache_Async_Working()
        {
            var client = _factory.CreateClient();

            var response = await client.GetStringAsync("/Cache/GetAsync");

            Assert.Equal(Constants.WasCachedResult, response);
        }
    }
}