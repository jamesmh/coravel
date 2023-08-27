using System;
using System.Collections.Generic;
using Coravel.Scheduling.Schedule.UtcTime;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule.Mutex;

public sealed class InMemoryMutex : IMutex
{
    private IUtcTime _utcTime;
    private readonly object _lock = new();
    private readonly Dictionary<string, MutexItem> _mutexCollection = new();

    public InMemoryMutex() => _utcTime = new SystemUtcTime();

    /// <summary>
    /// Used to override the default usage of DateTime.UtcNow.
    /// </summary>
    /// <param name="time"></param>
    public void Using(IUtcTime time)
    {
        _utcTime = time;
    }

    public void Release(string key)
    {
        lock (_lock)
        {
            if (_mutexCollection.TryGetValue(key, out var mutex))
            {
                mutex.Locked = false;
                mutex.ExpiresAt = null;
            }
        }
    }

    public bool TryGetLock(string key, int timeoutMinutes)
    {
        lock (_lock)
        {
            if (_mutexCollection.TryGetValue(key, out var mutex))
            {
                if (mutex.Locked)
                {
                    if (_utcTime.Now >= mutex.ExpiresAt)
                    {
                        return CreateLockedMutex(key, timeoutMinutes);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return CreateLockedMutex(key, timeoutMinutes);
                }

            }
            else
            {
                return CreateLockedMutex(key, timeoutMinutes);
            }
        }
    }

    private bool CreateLockedMutex(string key, int timeoutMinutes)
    {
        DateTime? expiresAt = _utcTime.Now.AddMinutes(timeoutMinutes);

        if (_mutexCollection.TryGetValue(key, out var mutex))
        {
            mutex.Locked = true;
            mutex.ExpiresAt = expiresAt;
        }
        else
        {
            _mutexCollection.Add(key, new MutexItem
            {
                Locked = true,
                ExpiresAt = expiresAt
            });
        }

        return true;
    }

    private sealed class MutexItem
    {
        public bool Locked { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}