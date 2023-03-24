using Coravel.Events.Interfaces;

namespace CoravelUnitTests.Events.EventsAndListeners
{
    public class TestEventWithDispatcher : IEvent
    {
        public IDispatcher Dispatcher { get; set; }
        
        public TestEventWithDispatcher(IDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
        }
    }
}