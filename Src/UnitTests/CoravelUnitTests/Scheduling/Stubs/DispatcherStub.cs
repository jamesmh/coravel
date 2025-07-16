using System.Threading.Tasks;
using Coravel.Events.Interfaces;

namespace CoravelUnitTests.Scheduling.Stubs
{
    public class DispatcherStub : IDispatcher
    {
        public Task Broadcast(IEvent toBroadcast)
        {
            return Task.CompletedTask;
        }
    }
}