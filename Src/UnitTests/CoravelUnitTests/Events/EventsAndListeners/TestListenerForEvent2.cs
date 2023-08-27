using System;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;

namespace CoravelUnitTests.Events.EventsAndListeners;

public class TestListenerForEvent2 : IListener<TestEvent2>
{
    private readonly Action _a;
    public TestListenerForEvent2(Action a) => _a = a;

    public Task HandleAsync(TestEvent2 dipatchedEvent)
    {
        _a();
        return Task.CompletedTask;
    }
}