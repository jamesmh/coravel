namespace Coravel.Scheduling.Schedule.Interfaces
{
    public interface IScheduleInterval
    {
        IScheduleRestriction EveryMinute();
        IScheduleRestriction EveryFiveMinutes();
        IScheduleRestriction EveryTenMinutes();
        IScheduleRestriction EveryFifteenMinutes();
        IScheduleRestriction EveryThirtyMinutes();
        IScheduleRestriction Hourly();
        IScheduleRestriction HourlyAt(int minute);
        IScheduleRestriction Daily();
        IScheduleRestriction DailyAtHour(int hour);
        IScheduleRestriction DailyAt(int hour, int minute);
        IScheduleRestriction Weekly();
    }
}