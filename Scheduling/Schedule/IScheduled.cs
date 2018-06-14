namespace Scheduling.Schedule
{
    public interface IScheduled
    {
        void EveryMinute();
        void EachMinutes(int minutes);
        void EveryHour();
        void Daily();
    }
}