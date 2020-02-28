using System;

namespace Coravel.Scheduling.Schedule.Cron
{
    public interface ICronExpression
    {
        /// <summary>
        /// Appends a Day of Week to our cron expression.
        /// </summary>
        /// <param name="day">Day to append.</param>
        /// <returns></returns>
        ICronExpression AppendWeekDay(DayOfWeek day);
        
        /// <summary>
        /// Checks if the provided DateTime parameter is due.
        /// </summary>
        /// <param name="time">DateTime to check.</param>
        /// <returns></returns>
        bool IsDue(DateTime time);
        
        /// <summary>
        /// Checks if the provided DateTime parameter is due.
        /// </summary>
        /// <param name="time">DateTime to check.</param>
        /// <returns></returns>
        bool IsWeekDayDue(DateTime time);
    }
}