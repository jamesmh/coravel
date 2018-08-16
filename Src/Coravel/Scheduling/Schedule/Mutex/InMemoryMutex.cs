using System;
using System.Collections.Generic;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule.Mutex
{
    public class InMemoryMutex : IMutex
    {
        private object _lock = new object();
        private Dictionary<string, MutexItem> _mutexCollection = new Dictionary<string, MutexItem>();

        public void Release(string key)
        {
            lock (this._lock)
            {
                if (!this._mutexCollection.ContainsKey(key))
                {
                    this._mutexCollection[key] = new MutexItem
                    {
                        Locked = false,
                        ExpiresAt = null
                    };
                }
            }
        }

        public bool TryGetLock(string key, int timeoutMinutes)
        {
            lock (this._lock)
            {
                if (!this._mutexCollection.ContainsKey(key))
                {
                    return this.CreateLockedMutex(key, timeoutMinutes);
                }
                else
                {
                    var mutex = this._mutexCollection[key];

                    if (mutex.Locked)
                    {
                        if (DateTime.UtcNow >= mutex.ExpiresAt)
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
            }
        }

        private bool CreateLockedMutex(string key, int timeoutMinutes)
        {
            DateTime? expiresAt = DateTime.UtcNow.AddMinutes(timeoutMinutes);
            var mutex = new MutexItem
            {
                Locked = true,
                ExpiresAt = expiresAt
            };

            if (this._mutexCollection.ContainsKey(key))
            {
                this._mutexCollection.Add(key, mutex);
            }
            else
            {
                this._mutexCollection[key] = mutex;
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