using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Cache;

public class RememberTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
{
    private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

    public RememberTests(WebApplicationFactory<TestMvcApp.Startup> factory) => _factory = factory;

    [Fact]
    public async Task Remember_Cache_Working()
    {
        var client = _factory.CreateClient();

        var response = await client.GetStringAsync("/Cache/Remember");
        var responseTwo = await client.GetStringAsync("/Cache/Remember");
        var responseThree = await client.GetStringAsync("/Cache/Remember");

        Assert.Equal(Constants.WasCachedResult, response);
        Assert.Equal(Constants.NotCachedResult, responseTwo);
        Assert.Equal(Constants.NotCachedResult, responseThree);
    }
}
