using System;

namespace Coravel.Scheduling.Schedule
{
    /// <summary>
    /// Data representation of a scheduled event.
    /// </summary>
    public class ScheduleInfo
    {
        /// <summary>
        /// The CRON expression for the schedule.
        /// </summary>
        public string CronExpression { get; }

        /// <summary>
        /// Indicates if the schedule is scheduled per second.
        /// </summary>
        public bool IsScheduledPerSecond { get; }

        /// <summary>
        /// The second interval (not included in the CRON expression so exposed separately).
        /// </summary>
        public int? SecondsInterval { get; }

        /// <summary>
        /// The type of the invocable.
        /// </summary>
        public Type InvocableType { get; }

        /// <summary>
        /// Indicates if overlapping should be prevented.
        /// </summary>
        public bool PreventOverlapping { get; }

        /// <summary>
        /// The unique identifier for the event.
        /// </summary>
        public string EventUniqueId { get; }

        /// <summary>
        /// Indicates if the schedule has any "when" predicates.
        /// </summary>
        public bool HasWhenPredicates { get; }

        /// <summary>
        /// The time zone information for the schedule.
        /// </summary>
        public TimeZoneInfo ZonedTimeZone { get; }

        /// <summary>
        /// Indicates if the schedule should run once at start.
        /// </summary>
        public bool RunOnceAtStart { get; }

        /// <summary>
        /// Indicates if the schedule should run only once.
        /// </summary>
        public bool RunOnce { get; }

        public ScheduleInfo(
            string cronExpression,
            bool isScheduledPerSecond,
            int? secondsInterval,
            Type invocableType,
            bool preventOverlapping,
            string eventUniqueId,
            bool hasWhenPredicates,
            TimeZoneInfo zonedTimeZone,
            bool runOnceAtStart,
            bool runOnce)
        {
            CronExpression = cronExpression;
            IsScheduledPerSecond = isScheduledPerSecond;
            SecondsInterval = secondsInterval;
            InvocableType = invocableType;
            PreventOverlapping = preventOverlapping;
            EventUniqueId = eventUniqueId;
            HasWhenPredicates = hasWhenPredicates;
            ZonedTimeZone = zonedTimeZone;
            RunOnceAtStart = runOnceAtStart;
            RunOnce = runOnce;
        }
    }
}