namespace Coravel.Scheduling.Schedule.Interfaces;

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
    /// <param name="key">The key that identifies the lock.</param>
    /// <param name="timeoutMinutes">The maximum number of minutes to wait for the lock.</param>
    /// <returns></returns>
    bool TryGetLock(string key, int timeoutMinutes);

    /// <summary>
    /// Release the hold of a lock for a given key.
    /// </summary>
    /// <param name="key">The key that identifies the lock.</param>
    void Release(string key);
}