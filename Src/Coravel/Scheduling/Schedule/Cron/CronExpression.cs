using System;
using System.Collections.Generic;

namespace Coravel.Scheduling.Schedule.Cron
{
    public class CronExpression
    {
        private string _minutes;
        private string _hours;
        private string _days;
        private string _months;
        private string _weekdays;
        private readonly TimeZoneInfo _timeZoneInfo;

        public CronExpression(string expression, TimeZoneInfo timeZoneInfo = null)
        {
            var values = expression.Split(' ');
            if (values.Length != 5)
            {
                throw new Exception($"Cron expression '{expression}' is malformed.");
            }

            this._minutes = values[0];
            this._hours = values[1];
            this._days = values[2];
            this._months = values[3];
            this._weekdays = values[4];
            this._timeZoneInfo = timeZoneInfo;
        }

        public CronExpression AppendWeekDay(DayOfWeek day)
        {
            int intDay = (int)day;

            if (this._weekdays == "*")
            {
                this._weekdays = intDay.ToString();
            }
            else
            {
                this._weekdays += "," + intDay.ToString();
            }

            return this;
        }

        public bool IsDue(DateTime time)
        {
            if (_timeZoneInfo != null)
            {
                time = TimeZoneInfo.ConvertTimeFromUtc(time, _timeZoneInfo);
            }
            
            return this.IsMinuteDue(time)
                && this.IsHoursDue(time)
                && this.IsDayDue(time)
                && this.IsMonthDue(time)
                && this.IsWeekDayDue(time);
        }

        public bool IsWeekDayDue(DateTime time)
        {
            if (_timeZoneInfo != null)
            {
                time = TimeZoneInfo.ConvertTimeFromUtc(time, _timeZoneInfo);
            }
            
            return new CronExpressionPart(this._weekdays, 7).IsDue((int)time.DayOfWeek);
        }

        private bool IsMinuteDue(DateTime time)
        {
            return new CronExpressionPart(this._minutes, 60).IsDue(time.Minute);
        }

        private bool IsHoursDue(DateTime time)
        {
            return new CronExpressionPart(this._hours, 24).IsDue(time.Hour);
        }

        private bool IsDayDue(DateTime time)
        {
            return new CronExpressionPart(this._days, 31).IsDue(time.Day);
        }

        private bool IsMonthDue(DateTime time)
        {
            return new CronExpressionPart(this._months, 12).IsDue(time.Month);
        }
    }
}