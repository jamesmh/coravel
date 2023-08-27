using System.Data.Common;

namespace Coravel.Cache.Database.Core
{
    /// <summary>
    /// Defines the interface for a database driver that supports caching operations.
    /// </summary>
    public interface IDriver
    {
        /// <summary>
        /// Gets the name of the table that stores the cache entries.
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Gets the SQL statement that deletes a cache entry by its key.
        /// </summary>
        string DeleteByKeySQL { get; }

        /// <summary>
        /// Gets the SQL statement that inserts or updates a cache entry.
        /// </summary>
        string InsertOrUpdateCacheEntrySQL { get; }

        /// <summary>
        /// Gets the SQL statement that retrieves a cache entry by its key.
        /// </summary>
        string GetCacheEntrySQL { get; }

        /// <summary>
        /// Gets the SQL statement that creates the necessary tables for caching.
        /// </summary>
        string CreateTablesSQL { get; }

        /// <summary>
        /// Creates a database connection using the given connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>A database connection.</returns>
        DbConnection CreateConnection(string connectionString);
    }
}