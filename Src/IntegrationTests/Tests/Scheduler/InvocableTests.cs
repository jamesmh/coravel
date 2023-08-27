using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Scheduler;

public class InvocableTests : IClassFixture<WebApplicationFactory<TestMvcApp.Startup>>
{
    private readonly WebApplicationFactory<TestMvcApp.Startup> _factory;

    public InvocableTests(WebApplicationFactory<TestMvcApp.Startup> factory) => _factory = factory;

    [Fact]
    public async Task InvocableWithDIWorking()
    {
        var result = await _factory.CreateClient().GetAsync("/Invocable/RunInvocableScheduledTask");
        
        Assert.NotNull(result);
    }
}