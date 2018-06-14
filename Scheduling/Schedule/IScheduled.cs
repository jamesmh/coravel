namespace Scheduling.Schedule
{
    public interface IScheduled
    {
        void AfterMinutes(int minutes);
        void EveryMinute();
        void EveryFiveMinutes();
        void EveryTenMinutes();
        void EveryFifteenMinutes();
        void EveryThirtyMinutes();
        void Hourly();
        void Daily();
        void Weekly();
    }
}