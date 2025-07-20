using Coravel.Invocable;
using Coravel.Invocable.Interfaces;

namespace CoravelUnitTests.Scheduling.Stubs
{
    public class GlobalConfigurationStub : ICoravelGlobalConfiguration
    {
        public void RegisterPreventOverlapping<TInvocable>(string uniqueIdentifier) where TInvocable : IInvocable
        {
            // No-op for testing
        }

        public bool TryGetPreventOverlapping<TInvocable>(out string uniqueIdentifier) where TInvocable : IInvocable
        {
            uniqueIdentifier = null;
            return false;
        }
    }
}