using Coravel.Cache.Database.Core;
using Coravel.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel.Cache.SQLServer
{
    /// <summary>
    /// Provides extension methods for registering and configuring SQL Server cache services.
    /// </summary>
    public static class ServiceRegistration
    {
        /// <summary>
        /// Adds the SQL Server cache services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="connectionString">The connection string for the SQL Server database.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddSQLServerCache(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<ICache>(new DatabaseCache(connectionString, new SqlServerDriver()));
            return services;
        }
    }

}