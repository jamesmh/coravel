namespace Coravel.Scheduling.Schedule.Interfaces
{
    /// <summary>
    /// Abstraction for locking scheduled events so 
    /// they may detect overlapping executions etc.
    /// </summary>
    public interface IMutex
    {
        /// <summary>
        /// Attempt to get a lock for the given key. 
        /// If true, lock is available and was granted.
        /// If false, lock is still in use by another.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timeoutMinutes"></param>
        /// <returns></returns>
        bool TryGetLock(string key, int timeoutMinutes);
        
        /// <summary>
        /// Release the hold of a lock for a given key.
        /// </summary>
        /// <param name="key"></param>
        void Release(string key);
    }
}