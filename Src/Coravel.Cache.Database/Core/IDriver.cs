using System.Data.Common;
using System.Data.SqlClient;

namespace Coravel.Cache.Database
{
    public interface IDriver
    {
        string TableName { get; }
        string DeleteByKeySQL { get; }
        string InsertOrUpdateCacheEntrySQL { get; }
        string GetCacheEntrySQL { get; }
        string CreateTablesSQL { get; }
        DbConnection CreateConnection(string connectionString);
    }
}