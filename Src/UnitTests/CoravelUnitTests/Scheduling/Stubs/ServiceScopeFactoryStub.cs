using System;
using Microsoft.Extensions.DependencyInjection;

namespace CoravelUnitTests.Scheduling.Stubs
{
    public class ServiceScopeFactoryStub : IServiceScopeFactory
    {
        public IServiceScope CreateScope()
        {
            return new AsyncServiceScope(new ServiceScopeStub());
        }
    }

    public class ServiceScopeStub : IServiceScope
    {
        public IServiceProvider ServiceProvider => throw new NotImplementedException();

        public void Dispose()
        {
            // no-op
        }
    }
}