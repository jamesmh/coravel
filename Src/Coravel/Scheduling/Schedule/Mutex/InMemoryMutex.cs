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
        private Dictionary<string, MutexItem> _mutexCollection = new Dictionary<string, MutexItem>();

        public InMemoryMutex() {
            this._utcTime = new SystemUtcTime();
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
                if (!this._mutexCollection.TryGetValue(key, out var mutex))
                {
                    return this.CreateLockedMutex(key, timeoutMinutes);
                }
                if (!mutex.Locked)
                {
                    return this.CreateLockedMutex(key, timeoutMinutes);
                }

                if (this._utcTime.Now >= mutex.ExpiresAt)
                {
                    return this.CreateLockedMutex(key, timeoutMinutes);
                }
                return false;
            }
        }

        private bool CreateLockedMutex(string key, int timeoutMinutes)
        {
            var expiresAt = this._utcTime.Now.AddMinutes(timeoutMinutes);

            if (this._mutexCollection.TryGetValue(key, out var mutex))
            {
                mutex.Locked = true;
                mutex.ExpiresAt = expiresAt;
                return true;
            }

            this._mutexCollection.Add(key, new MutexItem
            {
                Locked = true,
                ExpiresAt = expiresAt
            });
            return true;
        }

        private class MutexItem
        {
            public bool Locked { get; set; }
            public DateTime? ExpiresAt { get; set; }
        }
    }
}