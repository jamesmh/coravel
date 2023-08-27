using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Cache;

public class ForeverAsyncTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
{
    private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

    public ForeverAsyncTests(WebApplicationFactory<TestMvcApp.Startup> factory) => _factory = factory;

    [Fact]
    public async Task Forever_Async_Cache_Working()
    {
        var client = _factory.CreateClient();

        var response = await client.GetStringAsync("/Cache/ForeverAsync");
        var responseTwo = await client.GetStringAsync("/Cache/ForeverAsync");
        var responseThree = await client.GetStringAsync("/Cache/ForeverAsync");

        Assert.Equal(Constants.WasCachedResult, response);
        Assert.Equal(Constants.NotCachedResult, responseTwo);
        Assert.Equal(Constants.NotCachedResult, responseThree);
    }
}
