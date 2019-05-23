using Coravel.Cache.Database;
using Coravel.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel.Cache.SqlServer
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddSqlServerCache(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<ICache>(new DatabaseCache(connectionString, new SqlServerDriver()));
            return services;
        }
    }
}