using System;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;

namespace UnitTests.Events.EventsAndListeners
{
    public class TestListener1ForEvent1 : IListener<TestEvent1>
    {
        private Action _a;
        public TestListener1ForEvent1(Action a){
            this._a = a;
        }

        public Task<bool> HandleAsync(TestEvent1 dipatchedEvent)
        {
            this._a();
            return Task.FromResult(true);
        }
    }
}