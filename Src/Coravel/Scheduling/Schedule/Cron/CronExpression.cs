using System;

namespace Coravel.Scheduling.Schedule.Cron;

public sealed class CronExpression
{
    private readonly string _minutes;
    private readonly string _hours;
    private readonly string _days;
    private readonly string _months;
    private string _weekdays;

    public CronExpression(string expression)
    {
        var values = expression.Split(' ');
        if (values.Length != 5)
        {
            throw new MalformedCronExpressionException($"Cron expression '{expression}' is malformed.");
        }

        _minutes = values[0];
        _hours = values[1];
        _days = values[2];
        _months = values[3];
        _weekdays = values[4];

        GuardExpressionIsValid();
    }

    public CronExpression AppendWeekDay(DayOfWeek day)
    {
        int intDay = (int)day;

        if (_weekdays == "*")
        {
            _weekdays = intDay.ToString();
        }
        else
        {
            _weekdays += "," + intDay.ToString();
        }

        return this;
    }

    public bool IsDue(DateTime time)
    {
        return IsMinuteDue(time)
               && IsHoursDue(time)
               && IsDayDue(time)
               && IsMonthDue(time)
               && IsWeekDayDue(time);
    }

    public bool IsWeekDayDue(DateTime time)
    {
        return new CronExpressionPart(_weekdays, 7).IsDue((int)time.DayOfWeek);
    }

    private bool IsMinuteDue(DateTime time)
    {
        return new CronExpressionPart(_minutes, 60).IsDue(time.Minute);
    }

    private bool IsHoursDue(DateTime time)
    {
        return new CronExpressionPart(_hours, 24).IsDue(time.Hour);
    }

    private bool IsDayDue(DateTime time)
    {
        return new CronExpressionPart(_days, 31).IsDue(time.Day);
    }

    private bool IsMonthDue(DateTime time)
    {
        return new CronExpressionPart(_months, 12).IsDue(time.Month);
    }

    private void GuardExpressionIsValid()
    {
        // We don't want to check that the expression is due, but just run validation and ignore any results.
        var time = DateTime.UtcNow;
        IsMinuteDue(time);
        IsHoursDue(time);
        IsDayDue(time);
        IsMonthDue(time);
        IsWeekDayDue(time);
    }
}
