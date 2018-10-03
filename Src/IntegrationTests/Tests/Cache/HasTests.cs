using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Cache
{
    public class HasTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
    {
        private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

        public HasTests(WebApplicationFactory<TestMvcApp.Startup> factory)
        {
            this._factory = factory;
        }

        [Fact]
        public async Task Has_Cache_Working()
        {
            var client = _factory.CreateClient();

            var response = await client.GetStringAsync("/Cache/Has");

            Assert.Equal(Constants.WasCachedResult, response);
        }
    }
}