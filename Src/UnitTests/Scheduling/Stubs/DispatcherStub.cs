using System.Threading.Tasks;
using Coravel.Events.Interfaces;

namespace UnitTests.Scheduling.Stubs
{
    public class DispatcherStub : IDispatcher
    {
        public Task Broadcast<TEvent>(TEvent toBroadcast) where TEvent : IEvent
        {
            return Task.CompletedTask;
        }
    }
}