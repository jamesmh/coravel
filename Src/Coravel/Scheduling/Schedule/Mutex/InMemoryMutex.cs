using System;
using System.Collections.Generic;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Scheduling.Schedule.UtcTime;

namespace Coravel.Scheduling.Schedule.Mutex
{
    public class InMemoryMutex : IMutex
    {
        private IUtcTime _utcTime;
        private readonly object _lock = new object();
        private readonly Dictionary<string, MutexItem> _mutexCollection;

        public InMemoryMutex() {
            this._utcTime = new SystemUtcTime();
            _mutexCollection = new Dictionary<string, MutexItem>();
        }

        /// <summary>
        /// Used to override the default usage of DateTime.UtcNow.
        /// </summary>
        /// <param name="time"></param>
        public void Using(IUtcTime time) {
            this._utcTime = time;
        }

        public void Release(string key)
        {
            lock (this._lock)
            {
                if (this._mutexCollection.TryGetValue(key, out var mutex))
                {
                    mutex.Locked = false;
                    mutex.ExpiresAt = null;
                }
            }
        }

        public bool TryGetLock(string key, int timeoutMinutes)
        {
            lock (this._lock)
            {
                if (this._mutexCollection.TryGetValue(key, out var mutex))
                {
                    if (mutex.Locked)
                    {
                        if (this._utcTime.Now >= mutex.ExpiresAt)
                        {
                            return this.CreateLockedMutex(key, timeoutMinutes);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return this.CreateLockedMutex(key, timeoutMinutes);
                    }

                }
                else
                {
                    return this.CreateLockedMutex(key, timeoutMinutes);
                }
            }
        }

        private bool CreateLockedMutex(string key, int timeoutMinutes)
        {
            DateTime? expiresAt = this._utcTime.Now.AddMinutes(timeoutMinutes);

            if (this._mutexCollection.TryGetValue(key, out var mutex))
            {
                mutex.Locked = true;
                mutex.ExpiresAt = expiresAt;
            }
            else
            {
                this._mutexCollection.Add(key, new MutexItem
                {
                    Locked = true,
                    ExpiresAt = expiresAt
                });
            }

            return true;
        }

        private class MutexItem
        {
            public bool Locked { get; set; }
            public DateTime? ExpiresAt { get; set; }
        }
    }
}