using System;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;
using Demo.Events;

namespace Demo.Listeners
{
    public class WriteMessageToConsoleListener : IListener<DemoEvent>
    {
        public Task HandleAsync(DemoEvent broadcasted)
        {
            Console.WriteLine($"WriteMessageToConsoleListener receive this message from DemoEvent: ${broadcasted.Message}");
            return Task.CompletedTask;
        }
    }
}