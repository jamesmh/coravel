using System.Threading.Tasks;
using Coravel.Events.Interfaces;

namespace CoravelUnitTests.Events.EventsAndListeners
{
    public class TestListenerThatFiresEvent1And2 : IListener<TestEventWithDispatcher>
    {
        public async Task HandleAsync(TestEventWithDispatcher broadcasted)
        {
            await broadcasted.Dispatcher.Broadcast(new TestEvent1());
            await broadcasted.Dispatcher.Broadcast(new TestEvent2());
        }
    }
}