using Coravel.Cache.Database.Core;
using Coravel.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel.Cache.PostgreSQL
{
    public static class ServiceRegistration
    {

        public static IServiceCollection AddPostgreSQLCache(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<ICache>(new DatabaseCache(connectionString, new PostgreSQLDriver()));
            return services;
        }
    }
}