using System;
using System.Data.Common;
using System.Threading.Tasks;
using Coravel.Cache.Interfaces;
using Dapper;
using Newtonsoft.Json;

namespace Coravel.Cache.Database.Core
{
    public class DatabaseCache : ICache
    {
        private readonly IDriver _driver;
        private readonly string _connectionString;
        private static bool _TableCreated = false;       

        public DatabaseCache(string connectionString, IDriver driver)
        {
            this._connectionString = connectionString;
            this._driver = driver;
            this.TryCreateCacheTablesIfNotExisting();
        }

        public void Flush()
        {
            this._connectionString.AsDBConnection(this._driver, con =>            
                con.Execute($@"DELETE FROM {this._driver.TableName};")
            );
        }

        public TResult Forever<TResult>(string key, Func<TResult> cacheFunc)
        {
            return this.TryCachingEntry(key, cacheFunc, DateTimeOffset.MaxValue);
        }

        public async Task<TResult> ForeverAsync<TResult>(string key, Func<Task<TResult>> cacheFunc)
        {
            return await this.TryCachingEntryAsync(key, cacheFunc, DateTimeOffset.MaxValue);
        }

        public void Forget(string key)
        {
            this._connectionString.AsDBConnection(this._driver, con =>            
                con.Execute($"DELETE FROM {this._driver.TableName} WHERE Key = @Key", new { Key = key })
            );
        }

        public TResult Remember<TResult>(string key, Func<TResult> cacheFunc, TimeSpan expiresIn)
        {
            return this.TryCachingEntry(key, cacheFunc, DateTimeOffset.UtcNow.Add(expiresIn));  
        }

        public async Task<TResult> RememberAsync<TResult>(string key, Func<Task<TResult>> cacheFunc, TimeSpan expiresIn)
        {
            return await this.TryCachingEntryAsync(key, cacheFunc, DateTimeOffset.UtcNow.Add(expiresIn));
        }

                public async Task<bool> HasAsync(string key)
        {
            return await this._connectionString.AsDBTransactionAsync<bool>(this._driver, async (con, trans) =>
            {
                var cachedItem = await GetCacheItemAsync(key, con, trans);
                
                if(cachedItem == null)
                {
                    return false;
                }

                if(cachedItem.IsExpired())
                {
                    return false;
                }

                return true;
            });
        }

        public async Task<TResult> GetAsync<TResult>(string key)
        {
            return await this._connectionString.AsDBTransactionAsync<TResult>(this._driver, async (con, trans) =>
            {
                var cachedItem = await GetCacheItemAsync(key, con, trans);

                if(cachedItem == null)
                {
                    throw new NoCacheEntryException("Cache entry for key \"" + key + "\" does not exist.");
                }

                if(cachedItem.IsExpired())
                {
                    throw new NoCacheEntryException("Cache entry for key \"" + key + "\" has expired.");
                }

                return JsonConvert.DeserializeObject<TResult>(cachedItem.Value);
            });
        }

        private T TryCachingEntry<T>(string key, Func<T> cacheFunc, DateTimeOffset expiresAt)
        {
            return this._connectionString.AsDBTransaction<T>(this._driver, (con, trans) =>
            {
                var cachedItem = GetCacheItem(key, con, trans);

                if (cachedItem != null)
                {
                    bool expired = cachedItem.ExpiresAt.UtcDateTime <= DateTimeOffset.UtcNow;
                    if (expired)
                    {
                        con.Execute(this._driver.DeleteByKeySQL, new { Key = key }, trans);                        
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<T>(cachedItem.Value);
                    }
                }

                T result = cacheFunc();

                object parameters = new
                {
                    Key = key,
                    Value = JsonConvert.SerializeObject(result),
                    ExpiresAt = expiresAt
                };

                con.Execute(this._driver.InsertOrUpdateCacheEntrySQL, parameters, trans);

                return result;
            });
        }

        private async Task<T> TryCachingEntryAsync<T>(string key, Func<Task<T>> cacheFunc, DateTimeOffset expiresAt)
        {
            return await this._connectionString.AsDBTransactionAsync<T>(this._driver, async (con, trans) =>
            {
                var cachedItem = await GetCacheItemAsync(key, con, trans);

                if (cachedItem != null)
                {
                    bool expired = cachedItem.IsExpired();
                    if (expired)
                    {
                        await con.ExecuteAsync(this._driver.DeleteByKeySQL, new { Key = key }, trans);                        
                    } 
                    else
                    {
                        return JsonConvert.DeserializeObject<T>(cachedItem.Value);
                    }
                }

                T result = await cacheFunc();

                object parameters = new
                {
                    Key = key,
                    Value = JsonConvert.SerializeObject(result),
                    ExpiresAt = expiresAt
                };

                await con.ExecuteAsync(this._driver.InsertOrUpdateCacheEntrySQL, parameters, trans);

                return result;
            });
        }

        private CacheEntryDBModel GetCacheItem(string key, DbConnection con, DbTransaction trans)
        {
            return con.QueryFirstOrDefault<CacheEntryDBModel>(this._driver.GetCacheEntrySQL, new { Key = key }, trans);
        }

        private async Task<CacheEntryDBModel> GetCacheItemAsync(string key, DbConnection con, DbTransaction trans)
        {
            return await con.QueryFirstOrDefaultAsync<CacheEntryDBModel>(this._driver.GetCacheEntrySQL, new { Key = key }, trans);
        }

        private void TryCreateCacheTablesIfNotExisting()
        {
            if (_TableCreated)
            {
                return;
            }

            this._connectionString.AsDBTransaction(this._driver, (con, trans) =>
            {
                con.Execute(this._driver.CreateTablesSQL, null, trans);
                _TableCreated = true;
            });
        }
    }
}