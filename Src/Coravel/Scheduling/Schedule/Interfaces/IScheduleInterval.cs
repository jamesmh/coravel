namespace Coravel.Scheduling.Schedule.Interfaces
{
    public interface IScheduleInterval
    {
        IScheduleRestriction AfterMinutes(int minutes);
        IScheduleRestriction EveryMinute();
        IScheduleRestriction EveryFiveMinutes();
        IScheduleRestriction EveryTenMinutes();
        IScheduleRestriction EveryFifteenMinutes();
        IScheduleRestriction EveryThirtyMinutes();
        IScheduleRestriction Hourly();
        IScheduleRestriction Daily();
        IScheduleRestriction Weekly();
    }
}