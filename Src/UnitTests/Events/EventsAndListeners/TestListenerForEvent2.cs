using System;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;

namespace UnitTests.Events.EventsAndListeners
{
    public class TestListenerForEvent2 : IListener<TestEvent2>
    {
        private Action _a;
        public TestListenerForEvent2(Action a){
            this._a = a;
        }

        public Task HandleAsync(TestEvent2 dipatchedEvent)
        {
            this._a();
            return Task.CompletedTask;
        }
    }
}