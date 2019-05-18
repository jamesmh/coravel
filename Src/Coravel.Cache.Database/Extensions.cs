using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Coravel.Cache.Database
{
    internal static class Extensions
    {
        public static async Task<T> AsDBConnectionAsync<T>(this string connectionString, Func<SqlConnection, Task<T>> func)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                return await func(con);
            }
        }

        public static async Task AsDBConnectionAsync(this string connectionString, Func<SqlConnection, Task> func)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                await con.OpenAsync();
                await func(con);
            }
        }

        public static T AsDBConnection<T>(this string connectionString, Func<SqlConnection, T> func)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                return func(con);
            }
        }

        public static void AsDBConnection(this string connectionString, Action<SqlConnection> func)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                func(con);
            }
        }

        public static async Task<T> AsDBTransactionAsync<T>(this string connectionString, Func<SqlConnection, SqlTransaction, Task<T>> func) => await connectionString.AsDBConnectionAsync(
                async con =>
                {
                    using (SqlTransaction trans = con.BeginTransaction())
                    {
                        T result = await func(con, trans);
                        trans.Commit();
                        return result;
                    }
                }
            );

        public static async Task AsDBTransactionAsync(this string connectionString, Func<SqlConnection, SqlTransaction, Task> func) => await connectionString.AsDBConnectionAsync(
                async con =>
                {
                    using (SqlTransaction trans = con.BeginTransaction())
                    {
                        await func(con, trans);
                        trans.Commit();
                    }
                }
            );

        public static T AsDBTransaction<T>(this string connectionString, Func<SqlConnection, SqlTransaction, T> func) => connectionString.AsDBConnection(
            con =>
            {
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    T result = func(con, trans);
                    trans.Commit();
                    return result;
                }
            }
        );

        public static void AsDBTransaction(this string connectionString, Action<SqlConnection, SqlTransaction> func) => connectionString.AsDBConnection(
             con =>
             {
                 using (SqlTransaction trans = con.BeginTransaction())
                 {
                     func(con, trans);
                     trans.Commit();
                 }
             }
        );
    }
}
