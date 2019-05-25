using Coravel.Cache.Database.Core;
using Coravel.Cache.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel.Cache.SQLServer
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddSQLServerCache(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<ICache>(new DatabaseCache(connectionString, new SQLServerDriver()));
            return services;
        }
    }
}