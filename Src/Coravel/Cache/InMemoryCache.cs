using System;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Coravel.Cache.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Coravel.Cache
{
    public class InMemoryCache : ICache
    {
        private IMemoryCache _cache;

        public InMemoryCache(IMemoryCache cache) => this._cache = cache;

        public TResult Remember<TResult>(string key, Func<TResult> cacheFunc, TimeSpan expiresIn)
        {
            return this._cache.GetOrCreate(key, entry =>
            {
               entry.AbsoluteExpiration = DateTimeOffset.Now.Add(expiresIn);
                return cacheFunc();
            });
        }

        public async Task<TResult> RememberAsync<TResult>(string key, Func<Task<TResult>> cacheFunc, TimeSpan expiresIn)
        {
            return await this._cache.GetOrCreateAsync(key, async entry =>
           {
               entry.AbsoluteExpiration = DateTimeOffset.Now.Add(expiresIn);
               return await cacheFunc();
           });
        }

        public TResult Forever<TResult>(string key, Func<TResult> cacheFunc)
        {
            return this._cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpiration = null;
                return cacheFunc();
            });
        }

        public async Task<TResult> ForeverAsync<TResult>(string key, Func<Task<TResult>> cacheFunc)
        {
            return await this._cache.GetOrCreateAsync(key, async entry =>
           {
               entry.AbsoluteExpiration = null;
               return await cacheFunc();
           });
        }
    }
}