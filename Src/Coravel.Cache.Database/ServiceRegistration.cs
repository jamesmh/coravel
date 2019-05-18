using Coravel.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel.Cache.Database
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddSqlServerCache(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<ICache>(new SqlServerCache(connectionString));
            return services;
        }
    }
}