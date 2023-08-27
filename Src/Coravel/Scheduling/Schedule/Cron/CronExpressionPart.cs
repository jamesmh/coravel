namespace Coravel.Scheduling.Schedule.Cron;

/// <summary>
/// Represents a cron expression "part" parser needed for determining when events ought to be due.
/// </summary>
internal sealed class CronExpressionPart
{
    /// <summary>
    /// The cron expression used to determine when events are due.
    /// </summary>
    private readonly string _expression;
    private readonly int _timeParts;

    public CronExpressionPart(string expression, int timeParts)
    {
        _expression = expression.Trim();
        _timeParts = timeParts;
    }

    /// <summary>
    /// Based on the cron expression, is this DateTime due?
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public bool IsDue(int time)
    {
        return CronExpressionPartIsDue(time, _expression, _timeParts);
    }

    /// <summary>
    /// Generic method to check if a part of the cron expression is due.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="expression"></param>
    /// <param name="replaceZeroWith"></param>
    /// <returns></returns>
    private static bool CronExpressionPartIsDue(int time, string expression, int replaceZeroWith)
    {
        if (expression == "*")
        {
            return true;
        }

        var isDivisibleUnit = expression.IndexOf("*/") > -1;

        if (isDivisibleUnit)
        {
            if (!int.TryParse(expression.Remove(0, 2), out var divisor))
            {
                throw new MalformedCronExpressionException($"Cron entry '{expression}' is malformed.");
            }

            if (divisor == 0)
            {
                throw new MalformedCronExpressionException($"Cron entry ${expression} is attempting division by zero.");
            }

            if (time == 0)
            {
                time = replaceZeroWith;
            }

            return time % divisor == 0;
        }
        else
        {
            return new CronExpressionComplexPart(expression).CheckIfTimeIsDue(time);
        }
    }
}