namespace Coravel.Scheduling.Schedule.Interfaces
{
    public interface IMutex
    {
        bool TryGetLock(string key, int timeoutMinutes);
        void Release(string key);
    }
}