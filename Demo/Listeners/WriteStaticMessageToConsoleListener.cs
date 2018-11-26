using System;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;
using Demo.Events;

namespace Demo.Listeners
{
    public class WriteStaticMessageToConsoleListener : IListener<DemoEvent>
    {
        public Task HandleAsync(DemoEvent broadcasted)
        {
            Console.WriteLine("Listener writing a static message.");
            return Task.CompletedTask;
        }
    }
}