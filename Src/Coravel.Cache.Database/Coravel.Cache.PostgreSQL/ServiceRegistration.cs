using Coravel.Cache.Database.Core;
using Coravel.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel.Cache.PostgreSQL
{
    /// <summary>
    /// Provides extension methods for registering and configuring PostgreSQL cache services.
    /// </summary>
    public static class ServiceRegistration
    {

        /// <summary>
        /// Adds the PostgreSQL cache services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="connectionString">The connection string for the PostgreSQL database.</param>
        /// <returns>The same service collection.</returns>
        public static IServiceCollection AddPostgreSQLCache(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<ICache>(new DatabaseCache(connectionString, new PostgreSqlDriver()));
            return services;
        }
    }
}