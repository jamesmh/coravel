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

        public InMemoryCache(IMemoryCache cache)
        {
            this._cache = cache;
            this._keys = new HashSet<string>();
        }

        public TResult Remember<TResult>(string key, Func<TResult> cacheFunc, TimeSpan expiresIn)
        {
            this._keys.Add(key);

            return this._cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpiration = DateTimeOffset.Now.Add(expiresIn);
                entry.RegisterPostEvictionCallback(this.EvictionCallback);
                return cacheFunc();
            });
        }

        public async Task<TResult> RememberAsync<TResult>(string key, Func<Task<TResult>> cacheFunc, TimeSpan expiresIn)
        {
            this._keys.Add(key);

            return await this._cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpiration = DateTimeOffset.Now.Add(expiresIn);
                entry.RegisterPostEvictionCallback(this.EvictionCallback);
                return await cacheFunc();
            });
        }

        public TResult Forever<TResult>(string key, Func<TResult> cacheFunc)
        {
            this._keys.Add(key);

            return this._cache.GetOrCreate(key, entry =>
            {
                entry.Priority = CacheItemPriority.NeverRemove;
                return cacheFunc();
            });
        }

        public async Task<TResult> ForeverAsync<TResult>(string key, Func<Task<TResult>> cacheFunc)
        {
            this._keys.Add(key);

            return await this._cache.GetOrCreateAsync(key, async entry =>
            {
                entry.Priority = CacheItemPriority.NeverRemove;
                return await cacheFunc();
            });
        }

        public void Forget(string key)
        {
            this._cache.Remove(key);
            this._keys.Remove(key);
        }

        public void Flush()
        {
            foreach (string key in this._keys)
            {
                this._cache.Remove(key);
            }

            this._keys.Clear();
        }

        private void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            if (reason == EvictionReason.Expired || reason == EvictionReason.Capacity || reason == EvictionReason.Removed)
            {
                this._keys.Remove(key.ToString());
            }
        }
    }
}