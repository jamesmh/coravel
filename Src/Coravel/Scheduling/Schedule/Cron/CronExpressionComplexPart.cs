namespace Coravel.Scheduling.Schedule.Cron;

internal sealed class CronExpressionComplexPart
{
    private readonly string _expression;

    public CronExpressionComplexPart(string expression) => _expression = expression;

    /// <summary>
    /// From the cron expression, get all the int values that are valid due times.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public bool CheckIfTimeIsDue(int time)
    {
        var isRange = _expression.IndexOf('-') > -1;
        var isDivisibleRange = isRange && _expression.IndexOf('/') > -1;
        var isDelineatedArray = _expression.IndexOf(',') > -1;

        if (isRange && isDelineatedArray)
        {
            throw new MalformedCronExpressionException($"Cron expression '{_expression}' has mixed entry type.");
        }

        if (isDivisibleRange)
        {
            return CheckDivisibleRange(_expression, time);
        }

        if (isRange)
        {
            return CheckRange(_expression, time);
        }
        else if (isDelineatedArray)
        {
            return CheckDelineatedArray(_expression, time);
        }
        else
        {
            return CheckIsSpecifiedInt(_expression, time);
        }
    }

    /// <summary>
    /// Get value from a cron expression with a single value.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    private static bool CheckIsSpecifiedInt(string expression, int toCheck)
    {
        var parsed = int.TryParse(expression, out var parsedValue);

        if (!parsed)
        {
            throw new MalformedCronExpressionException($"Cron entry '{expression}' is malformed.");
        }

        return parsedValue == toCheck;
    }

    /// <summary>
    /// Get values from cron expression like '5,4,3'.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    private static bool CheckDelineatedArray(string expression, int toCheck)
    {
        var delineatedValues = expression.Split(',');
        foreach (var val in delineatedValues)
        {
            if (!int.TryParse(val, out var parsedValue))
            {
                throw new MalformedCronExpressionException($"Cron entry '{expression}' is malformed.");
            }

            if (parsedValue == toCheck)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get values from cron expression range (e.g.true '5-10').
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    private bool CheckRange(string expression, int toCheck)
    {
        // e.g. "5-10"
        var range = expression.Split('-');
        var firstParsed = int.TryParse(range[0], out var first);
        var secondParsed = int.TryParse(range[1], out var second);

        if (!(firstParsed && secondParsed))
        {
            throw new MalformedCronExpressionException($"Cron expression ${expression} is malformed.");
        }

        return IsBetween(first, second, toCheck);
    }

    /// <summary>
    /// Get values from cron expression range that has a divisor (e.g.true '5-10/2').
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    private bool CheckDivisibleRange(string expression, int toCheck)
    {
        // e.g. "5-10/2"
        var splitExpression = expression.Split('/');
        var range = splitExpression[0].Split('-');
        var firstParsed = int.TryParse(range[0], out var first);
        var secondParsed = int.TryParse(range[1], out var second);
        var divisorParsed = int.TryParse(splitExpression[1], out var divisor);

        if (!(firstParsed && secondParsed && divisorParsed))
        {
            throw new MalformedCronExpressionException($"Cron expression ${expression} is malformed.");
        }

        return IsBetweenSkipping(first, second, divisor, toCheck);
    }

    /// <summary>
    /// Get all int between (including) min and max.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    private static bool IsBetween(int min, int max, int toCheck)
    {
        for (var i = min; i <= max; i++)
        {
            if (i == toCheck)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get all int between (including) min and max when the number is divisible by the divisor.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="skip"></param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    private static bool IsBetweenSkipping(int min, int max, int skip, int toCheck)
    {
        for (var i = min; i <= max; i += skip)
        {
            if (i == toCheck)
            {
                return true;
            }
        }

        return false;
    }
}
