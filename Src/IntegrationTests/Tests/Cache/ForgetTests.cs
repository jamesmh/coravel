using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Cache;

public class ForgetTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
{
    private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

    public ForgetTests(WebApplicationFactory<TestMvcApp.Startup> factory) => _factory = factory;

    [Fact]
    public async Task Forget_Cache_Working()
    {
        var client = _factory.CreateClient();

        var rememberResponse = await client.GetStringAsync("/Cache/Remember");
        var rememberAsyncResponse = await client.GetStringAsync("/Cache/RememberAsync");
        var foreverResponse = await client.GetStringAsync("/Cache/Forever");
        var foreverAsyncResponse = await client.GetStringAsync("/Cache/ForeverAsync");

        await client.GetAsync("/Cache/Forget?key=Remember");

        var rememberReponseTwo = await client.GetStringAsync("/Cache/Remember");
        var rememberAsyncResponseTwo = await client.GetStringAsync("/Cache/RememberAsync");
        var foreverResponseTwo = await client.GetStringAsync("/Cache/Forever");
        var foreverAsyncResponseTwo = await client.GetStringAsync("/Cache/ForeverAsync");

        Assert.Equal(Constants.WasCachedResult, rememberResponse);
        Assert.Equal(Constants.WasCachedResult, rememberAsyncResponse);
        Assert.Equal(Constants.WasCachedResult, foreverResponse);
        Assert.Equal(Constants.WasCachedResult, foreverAsyncResponse);

        // After Forget
        Assert.Equal(Constants.WasCachedResult, rememberReponseTwo);
        Assert.Equal(Constants.NotCachedResult, rememberAsyncResponseTwo);
        Assert.Equal(Constants.NotCachedResult, foreverResponseTwo);
        Assert.Equal(Constants.NotCachedResult, foreverAsyncResponseTwo);
    }
}