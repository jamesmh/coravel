using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Coravel.Cache.Database.Core
{
    internal static class Extensions
    {
        public static async Task<T> AsDBConnectionAsync<T>(this string connectionString, IDriver driver, Func<DbConnection, Task<T>> func)
        {
            using (DbConnection con = driver.CreateConnection(connectionString))
            {
                await con.OpenAsync();
                return await func(con);
            }
        }

        public static async Task AsDBConnectionAsync(this string connectionString, IDriver driver, Func<DbConnection, Task> func)
        {
             using (DbConnection con = driver.CreateConnection(connectionString))
            {
                await con.OpenAsync();
                await func(con);
            }
        }

        public static T AsDBConnection<T>(this string connectionString, IDriver driver, Func<DbConnection, T> func)
        {
             using (DbConnection con = driver.CreateConnection(connectionString))
            {
                con.Open();
                return func(con);
            }
        }

        public static void AsDBConnection(this string connectionString, IDriver driver, Action<DbConnection> func)
        {
             using (DbConnection con = driver.CreateConnection(connectionString))
            {
                con.Open();
                func(con);
            }
        }

        public static async Task<T> AsDBTransactionAsync<T>(this string connectionString, IDriver driver, Func<DbConnection, DbTransaction, Task<T>> func) => await connectionString.AsDBConnectionAsync(driver,
                async con =>
                {
                    using (DbTransaction trans = con.BeginTransaction())
                    {
                        T result = await func(con, trans);
                        trans.Commit();
                        return result;
                    }
                }
            );

        public static async Task AsDBTransactionAsync(this string connectionString, IDriver driver, Func<DbConnection, DbTransaction, Task> func) => await connectionString.AsDBConnectionAsync(driver,
                async con =>
                {
                    using (DbTransaction trans = con.BeginTransaction())
                    {
                        await func(con, trans);
                        trans.Commit();
                    }
                }
            );

        public static T AsDBTransaction<T>(this string connectionString, IDriver driver, Func<DbConnection, DbTransaction, T> func) => connectionString.AsDBConnection(driver,
            con =>
            {
                using (DbTransaction trans = con.BeginTransaction())
                {
                    T result = func(con, trans);
                    trans.Commit();
                    return result;
                }
            }
        );

        public static void AsDBTransaction(this string connectionString, IDriver driver, Action<DbConnection, DbTransaction> func) => connectionString.AsDBConnection(driver,
             con =>
             {
                 using (DbTransaction trans = con.BeginTransaction())
                 {
                     func(con, trans);
                     trans.Commit();
                 }
             }
        );
    }
}
