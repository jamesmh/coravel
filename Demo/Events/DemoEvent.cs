using Coravel.Events.Interfaces;

namespace Demo.Events
{
    public class DemoEvent : IEvent
    {
        public string Message { get; set; }

        public DemoEvent(string message)
        {
            this.Message = message;
        }
    }
}