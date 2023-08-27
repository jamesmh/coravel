using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Cache;

public class GetAsyncTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
{
    private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

    public GetAsyncTests(WebApplicationFactory<TestMvcApp.Startup> factory) => _factory = factory;

    [Fact]
    public async Task Get_Async_Cache_Working()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Cache/GetAsync");
        var responseTwo = await client.GetStringAsync("/Cache/Remember");
        var responseThree = await client.GetStringAsync("/Cache/GetAsync");

        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(Constants.WasCachedResult, responseTwo);
        Assert.Equal(Constants.WasCachedResult, responseThree);
    }
}
