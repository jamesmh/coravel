namespace Coravel.Scheduling.Schedule.Cron;

public sealed class MalformedCronExpressionException : System.Exception
{
    public MalformedCronExpressionException(string message) : base(message)
    {
    }
}
