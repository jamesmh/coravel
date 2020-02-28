using System;

namespace Coravel.Scheduling.Schedule.Cron
{
    public class TimeZonedCronExpression : ICronExpression
    {
        private readonly ICronExpression _cronExpression;
        private readonly TimeZoneInfo _timeZoneInfo;

        public TimeZonedCronExpression(ICronExpression cronExpression, TimeZoneInfo timeZoneInfo)
        {
            _cronExpression = cronExpression;
            _timeZoneInfo = timeZoneInfo;
        }

        public ICronExpression AppendWeekDay(DayOfWeek day)
        {
            return new TimeZonedCronExpression(_cronExpression.AppendWeekDay(day), _timeZoneInfo);
        }

        public bool IsDue(DateTime time)
        {
            if (_timeZoneInfo != null)
            {
                time = TimeZoneInfo.ConvertTimeFromUtc(time, _timeZoneInfo);
            }

            return _cronExpression.IsDue(time);
        }

        public bool IsWeekDayDue(DateTime time)
        {
            if (_timeZoneInfo != null)
            {
                time = TimeZoneInfo.ConvertTimeFromUtc(time, _timeZoneInfo);
            }

            return _cronExpression.IsDue(time);
        }
    }
}