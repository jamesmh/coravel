using Coravel.Events.Interfaces;

namespace UnitTests.Events.EventsAndListeners
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