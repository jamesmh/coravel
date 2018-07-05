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
    public class InMemoryCache : ICache, ICacheOptions
    {
        private IMemoryCache _cache;
        private ILogger<ICache> _logger;
        private static readonly TimeSpan ExpiresInOneMinute = TimeSpan.FromMinutes(1);

        public InMemoryCache(IMemoryCache cache) => this._cache = cache;

        public TResult Cache<TResult>(Expression<Func<TResult>> expression, TimeSpan expiresIn)
        {
            string key = GenerateHashKey(expression);
            bool wasCreated = false;

            var cachedItem = this._cache.GetOrCreate(key, entry =>
            {
                wasCreated = true;
                entry.SlidingExpiration = expiresIn;
                return expression.Compile().Invoke();
            });

            TryLogging(key, wasCreated);

            return cachedItem;
        }

        public TResult Cache<TResult>(Expression<Func<TResult>> expression) => this.Cache(expression, ExpiresInOneMinute);

        public async Task<TResult> CacheAsync<TResult>(Expression<Func<Task<TResult>>> expression) => await this.CacheAsync(expression, ExpiresInOneMinute);

        public async Task<TResult> CacheAsync<TResult>(Expression<Func<Task<TResult>>> expression, TimeSpan expiresIn)
        {
            string key = GenerateHashKey(expression);
              bool wasCreated = false;

             var cachedItem = await this._cache.GetOrCreateAsync(key, async entry =>
            {
                wasCreated = true;
                entry.SlidingExpiration = expiresIn;
                return await expression.Compile().Invoke();
            });

            TryLogging(key, wasCreated);

            return cachedItem;
        }

        public ICacheOptions WithLogger(ILogger<ICache> logger)
        {
            this._logger = logger;
            return this;
        }

        private static string GenerateHashKey<TResult>(Expression<Func<TResult>> expression)
        {
            using (var hasher = MD5.Create())
            {
                string expressionString = expression.Body.ToString();
                byte[] hashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(expressionString));
                return Encoding.UTF8.GetString(hashBytes);
            }
        }

        private void TryLogging(string key, bool wasCreated)
        {
            if (this._logger != null)
            {
                if (wasCreated)
                {
                    this._logger.LogInformation($"Created new cached item using generated key {key}.");
                }
                else
                {
                    this._logger.LogInformation($"Retrieved cached item using generated key {key}.");
                }
            }
        }
    }
}