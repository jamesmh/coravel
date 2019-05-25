
using System.Data.Common;
using System.Data.SqlClient;
using Coravel.Cache.Database.Core;

namespace Coravel.Cache.SQLServer
{
    public class SQLServerDriver : IDriver
    {
         public string TableName => "[dbo].[CoravelCacheStore]";
          public string DeleteByKeySQL => $@"
            DELETE FROM {TableName} WHERE [Key] = @Key;
        ";

        public string InsertOrUpdateCacheEntrySQL => $@"
            IF (SELECT 1 FROM {TableName} WHERE [Key] = @Key) = 1
                UPDATE {TableName}
                SET [Value] = @Value, [ExpiresAt] = @ExpiresAt
                WHERE [Key] = @Key;
            ELSE
                INSERT INTO {TableName} ([Key], [Value], [ExpiresAt]) VALUES (@Key, @Value, @ExpiresAt);            
        ";

        public string GetCacheEntrySQL => $@"
            SELECT [Key], [Value], [ExpiresAt] 
            FROM {TableName} 
            WHERE [Key] = @Key;
        ";

        public string CreateTablesSQL => $@"
            IF OBJECT_ID(N'dbo.CoravelCacheStore', N'U') IS NULL BEGIN
                CREATE TABLE {TableName} (
                    [Key] NVARCHAR(255) NOT NULL PRIMARY KEY,
                    [Value] NVARCHAR(MAX) NOT NULL,
                    [ExpiresAt] DATETIMEOFFSET NOT NULL
                );
            END;
        ";

        public DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}