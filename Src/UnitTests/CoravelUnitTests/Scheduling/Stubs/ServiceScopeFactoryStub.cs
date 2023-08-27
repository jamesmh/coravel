using Microsoft.Extensions.DependencyInjection;

namespace CoravelUnitTests.Scheduling.Stubs;

public class ServiceScopeFactoryStub : IServiceScopeFactory
{
    public IServiceScope CreateScope()
    {
        throw new System.NotImplementedException();
    }
}