using System;

namespace Coravel.Scheduling.Schedule.Cron
{
    public class MalformedCronExpressionException : Exception
    {
        public MalformedCronExpressionException(string message) : base(message)
        {
        }
    }
}
