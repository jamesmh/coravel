using System;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;

namespace CoravelUnitTests.Events.EventsAndListeners;

public class TestListener2ForEvent1 : IListener<TestEvent1>
{
    private readonly Action _a;
    public TestListener2ForEvent1(Action a) => _a = a;

    public Task HandleAsync(TestEvent1 dipatchedEvent)
    {
        _a();
        return Task.CompletedTask;
    }
}