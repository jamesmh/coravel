using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Coravel.Cache.Interfaces;
using Dapper;
using Newtonsoft.Json;

namespace Coravel.Cache.Database
{
    internal class SqlServerCache : ICache
    {
        private readonly string _connectionString;
        private static bool _TableCreated = false;
        private static readonly string TableName = "[dbo].[CoravelCacheStore]";

        public SqlServerCache(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void Flush()
        {
            this.TryCreateCacheTablesIfNotExisting();
            this._connectionString.AsDBConnection(con =>            
                con.Execute(@"DELETE FROM [dbo].[CoravelCacheStore];")
            );
        }

        public TResult Forever<TResult>(string key, Func<TResult> cacheFunc)
        {
            this.TryCreateCacheTablesIfNotExisting();
            return this.TryCachingEntry(key, cacheFunc, DateTimeOffset.MaxValue);
        }

        public async Task<TResult> ForeverAsync<TResult>(string key, Func<Task<TResult>> cacheFunc)
        {
            await this.TryCreateCacheTablesIfNotExistingAsync();
            return await this.TryCachingEntryAsync(key, cacheFunc, DateTimeOffset.MaxValue);
        }

        public void Forget(string key)
        {
            this.TryCreateCacheTablesIfNotExisting();
            this._connectionString.AsDBConnection(con =>            
                con.Execute($"DELETE FROM {TableName} WHERE [Key] = @Key", new { Key = key })
            );
        }

        public TResult Remember<TResult>(string key, Func<TResult> cacheFunc, TimeSpan expiresIn)
        {
            this.TryCreateCacheTablesIfNotExisting();
            return this.TryCachingEntry(key, cacheFunc, DateTimeOffset.UtcNow.Add(expiresIn));  
        }

        public async Task<TResult> RememberAsync<TResult>(string key, Func<Task<TResult>> cacheFunc, TimeSpan expiresIn)
        {
            await this.TryCreateCacheTablesIfNotExistingAsync();
            return await this.TryCachingEntryAsync(key, cacheFunc, DateTimeOffset.UtcNow.Add(expiresIn));
        }

        private T TryCachingEntry<T>(string key, Func<T> cacheFunc, DateTimeOffset expiresAt)
        {
            return this._connectionString.AsDBTransaction<T>((con, trans) =>
            {
                var cachedItem = GetCacheItem<T>(key, con, trans);

                if (cachedItem != null)
                {
                    return cachedItem;
                }

                T result = cacheFunc();

                object parameters = new
                {
                    Key = key,
                    Value = JsonConvert.SerializeObject(result),
                    ExpiresAt = expiresAt
                };

                con.Execute(InsertOrUpdateCacheEntrySQL, parameters);

                return result;
            });
        }

        private async Task<T> TryCachingEntryAsync<T>(string key, Func<Task<T>> cacheFunc, DateTimeOffset expiresAt)
        {
            return await this._connectionString.AsDBTransactionAsync<T>(async (con, trans) =>
            {
                var cachedItem = await GetCacheItemAsync<T>(key, con, trans);

                if (cachedItem != null)
                {
                    return cachedItem;
                }

                T result = await cacheFunc();

                object parameters = new
                {
                    Key = key,
                    Value = JsonConvert.SerializeObject(result),
                    ExpiresAt = expiresAt
                };

                await con.ExecuteAsync(InsertOrUpdateCacheEntrySQL, parameters);

                return result;
            });
        }

        private static T GetCacheItem<T>(string key, SqlConnection con, SqlTransaction trans)
        {
            return con.QuerySingle<T>(GetCacheEntrySQL, new { Key = key }, trans);
        }

        private static async Task<T> GetCacheItemAsync<T>(string key, SqlConnection con, SqlTransaction trans)
        {
            return await con.QuerySingleAsync<T>(GetCacheEntrySQL, new { Key = key }, trans);
        }

        private async Task TryCreateCacheTablesIfNotExistingAsync()
        {
            if (_TableCreated)
            {
                return;
            }

            await this._connectionString.AsDBTransactionAsync(async (con, trans) =>
            {
                await con.ExecuteAsync(CreateTablesSQL, null, trans);
                _TableCreated = true;
            });
        }

        private void TryCreateCacheTablesIfNotExisting()
        {
            if (_TableCreated)
            {
                return;
            }

            this._connectionString.AsDBTransaction((con, trans) =>
            {
                con.Execute(CreateTablesSQL, null, trans);
                _TableCreated = true;
            });
        }

        private readonly static string InsertOrUpdateCacheEntrySQL = $@"
            IF (SELECT 1 FROM {TableName} WHERE [Key] = @Key) = 1
                UPDATE {TableName}
                SET [Value] = @Value, [ExpiresAt] = @ExpiresAt
                WHERE [Key] = @Key;
            ELSE
                INSERT INTO {TableName} ([Key], [Value], [ExpiresAt]) VALUES (@Key, @Value, @ExpiresAt);            
        ";

        private readonly static string GetCacheEntrySQL = $@"
            SELECT [Key], [Value], [ExpiresAt] 
            FROM {TableName} 
            WHERE [Key] = @Key
                AND [ExpiresAt] >= SYSDATETIMEOFFSET();
        ";

        private readonly static string CreateTablesSQL = $@"
            IF OBJECT_ID(N'dbo.CoravelCacheStore', N'U') IS NULL BEGIN
                CREATE TABLE {TableName} (
                    [Key] NVARCHAR NOT NULL PRIMARY KEY,
                    [Value] NVARCHAR(MAX) NOT NULL,
                    [ExpiresAt] DATETIMEOFFSET NOT NULL
                );
            END;
        ";
    }
}