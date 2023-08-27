using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Cache;

public class ForeverTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
{
    private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

    public ForeverTests(WebApplicationFactory<TestMvcApp.Startup> factory) => _factory = factory;

    [Fact]
    public async Task Forever_Cache_Working()
    {
        var client = _factory.CreateClient();

        var response = await client.GetStringAsync("/Cache/Forever");
        var responseTwo = await client.GetStringAsync("/Cache/Forever");
        var responseThree = await client.GetStringAsync("/Cache/Forever");

        Assert.Equal(Constants.WasCachedResult, response);
        Assert.Equal(Constants.NotCachedResult, responseTwo);
        Assert.Equal(Constants.NotCachedResult, responseThree);
    }
}
