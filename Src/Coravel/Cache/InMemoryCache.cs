using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Cache.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Coravel.Cache
{
    public class InMemoryCache : ICache
    {
        private readonly IMemoryCache _cache;
        private readonly HashSet<string> _keys;
        // I could create a new type for a thread safe hashset.
        // But, the Flush() method needs to be safe in it's entire operation.
        private readonly object _setLock;
        
        public InMemoryCache(IMemoryCache cache)
        {
            this._cache = cache;
            this._keys = new HashSet<string>();
            this._setLock = new object();
        }

        public TResult Remember<TResult>(string key, Func<TResult> cacheFunc, TimeSpan expiresIn)
        {
            lock (this._setLock)
            {
                this._keys.Add(key);
            }

            return this._cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpiration = DateTimeOffset.Now.Add(expiresIn);
                entry.RegisterPostEvictionCallback(this.EvictionCallback);
                return cacheFunc();
            });
        }

        public async Task<TResult> RememberAsync<TResult>(string key, Func<Task<TResult>> cacheFunc, TimeSpan expiresIn)
        {
            lock (this._setLock)
            {
                this._keys.Add(key);
            }

            return await this._cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpiration = DateTimeOffset.Now.Add(expiresIn);
                entry.RegisterPostEvictionCallback(this.EvictionCallback);
                return await cacheFunc();
            });
        }

        public TResult Forever<TResult>(string key, Func<TResult> cacheFunc)
        {
            lock (this._setLock)
            {
                this._keys.Add(key);
            }

            return this._cache.GetOrCreate(key, entry =>
            {
                entry.Priority = CacheItemPriority.NeverRemove;
                return cacheFunc();
            });
        }

        public async Task<TResult> ForeverAsync<TResult>(string key, Func<Task<TResult>> cacheFunc)
        {
            lock (this._setLock)
            {
                this._keys.Add(key);
            }

            return await this._cache.GetOrCreateAsync(key, async entry =>
            {
                entry.Priority = CacheItemPriority.NeverRemove;
                return await cacheFunc();
            });
        }

        public Task<bool> HasAsync(string key)
        {
            bool hasValue = this._cache.TryGetValue(key, out var dummy);
            return Task.FromResult(hasValue);
        }

        public Task<TResult> GetAsync<TResult>(string key)
        {
            bool hasValue = this._cache.TryGetValue(key, out TResult value);
            if(!hasValue)
            {
                throw new NoCacheEntryException("Cache entry for key \"" + key + "\" does not exist.");
            }
            return Task.FromResult(value);
        }

        public void Forget(string key)
        {
            this._cache.Remove(key);

            lock (this._setLock)
            {
                this._keys.Remove(key);
            }
        }

        public void Flush()
        {
            // This is the main reason we need a lock vs. creating a thread-safe "hash set".
            // While iterating through these keys, it's possible (without a lock around this section)
            // that some other thread adds, removes, etc. a key concurrently. That would cause issues in the 
            // loop - index out of bound exceptions, fail to "really" flush all the keys, etc.
            lock (this._setLock)
            {
                foreach (string key in this._keys)
                {
                    this._cache.Remove(key);
                }

                this._keys.Clear();
            }
        }

        private void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            if (reason == EvictionReason.Expired || reason == EvictionReason.Capacity || reason == EvictionReason.Removed)
            {
                lock (this._setLock)
                {
                    this._keys.Remove(key.ToString());
                }
            }
        }
    }
}