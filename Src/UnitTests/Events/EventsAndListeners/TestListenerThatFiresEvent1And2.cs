using System.Threading.Tasks;
using Coravel.Events.Interfaces;

namespace UnitTests.Events.EventsAndListeners
{
    public class TestListenerThatFiresEvent1And2 : IListener<TestEventWithDispatcher>
    {
        public async Task<bool> HandleAsync(TestEventWithDispatcher broadcasted)
        {
            await broadcasted.Dispatcher.Broadcast(new TestEvent1());
            await broadcasted.Dispatcher.Broadcast(new TestEvent2());

            return true;
        }
    }
}