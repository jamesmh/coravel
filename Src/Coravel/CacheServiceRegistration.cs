using System;
using Coravel.Cache;
using Coravel.Cache.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel
{
    /// <summary>
    /// IServiceCollection extensions for registering Coravel's Caching.
    /// </summary>
    public static class CacheServiceRegistration
    {
        /// <summary>
        /// Register Coravel's in memory caching.
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddCache(provider =>
                new InMemoryCache(provider.GetService<IMemoryCache>())
            );
            return services;
        }

        /// <summary>
        /// Register Coravel's caching using the specified caching driver.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="driver"></param>
        public static IServiceCollection AddCache(this IServiceCollection services, ICache driver)
        {
            services.AddSingleton<ICache>(driver);
            return services;
        }

        /// <summary>
        /// Register Coravel's caching using the specified caching driver.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="driverFunc"></param>
        public static IServiceCollection AddCache(this IServiceCollection services, Func<IServiceProvider, ICache> driverFunc)
        {
            services.AddSingleton<ICache>(driverFunc);
            return services;
        }
    }
}