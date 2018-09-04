using System;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;

namespace UnitTests.Events.EventsAndListeners
{
    public class TestListener2ForEvent1 : IListener<TestEvent1>
    {
        private Action _a;
        public TestListener2ForEvent1(Action a){
            this._a = a;
        }

        public Task HandleAsync(TestEvent1 dipatchedEvent)
        {
            this._a();
            return Task.CompletedTask;
        }
    }
}