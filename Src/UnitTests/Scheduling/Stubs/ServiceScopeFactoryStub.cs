using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.Scheduling.Stubs
{
    public class ServiceScopeFactoryStub : IServiceScopeFactory
    {
        public IServiceScope CreateScope()
        {
            throw new System.NotImplementedException();
        }
    }
}