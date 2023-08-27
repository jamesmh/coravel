using System.Data.Common;
using Coravel.Cache.Database.Core;
using Npgsql;

namespace Coravel.Cache.PostgreSQL
{
    internal sealed class PostgreSqlDriver : IDriver
    {
        public string TableName => "public.CoravelCacheStore";

        public string DeleteByKeySQL => $@"
            DELETE FROM {TableName} WHERE Key = @Key;
        ";

        public string InsertOrUpdateCacheEntrySQL => $@"
            INSERT INTO {TableName} (Key, Value, ExpiresAt) VALUES (@Key, cast(@Value AS json), @ExpiresAt) 
            ON CONFLICT(Key) DO UPDATE SET Value = cast(@Value AS json), ExpiresAt = @ExpiresAt;          
        ";

        public string GetCacheEntrySQL => $@"
            SELECT Key, Value, ExpiresAt 
            FROM {TableName} 
            WHERE Key = @Key;
        ";

        public string CreateTablesSQL => $@"
            DO
            $do$
            BEGIN
            IF NOT EXISTS (SELECT 1 FROM pg_tables WHERE schemaname = 'public' AND tablename = 'coravelcachestore') THEN
                CREATE TABLE {TableName} (
                    Key VARCHAR(255) NOT NULL PRIMARY KEY,
                    Value JSONB NOT NULL,
                    ExpiresAt timestamp with time zone NOT NULL
                );
            END IF;
            END
            $do$
        ";

        public DbConnection CreateConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}