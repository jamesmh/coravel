using System;

namespace Coravel;

public static partial class EventServiceRegistration
{
    public class EventServiceRegistrationConfigureEventsException : Exception
    {
        public EventServiceRegistrationConfigureEventsException(string message) : base(message)
        {

        }
    }
}
