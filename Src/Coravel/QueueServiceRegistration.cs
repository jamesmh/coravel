using Coravel.Queuing;
using Coravel.Queuing.HostedService;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel
{
    /// <summary>
    /// IServiceCollection extensions for registering Coravel's Queuing.
    /// </summary>
    public static class QueueServiceRegistration
    {
        /// <summary>
        /// Register Coravel's queueing.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns></returns>
        public static IServiceCollection AddQueue(this IServiceCollection services)
        {
            services.AddSingleton<IQueue>(p =>
                new Queue(p.GetRequiredService<IServiceScopeFactory>())
            );
            services.AddHostedService<QueuingHost>();
            return services;
        }

        public static IQueueConfiguration ConfigureQueue(this IApplicationBuilder app)
        {
            var queue = app.ApplicationServices.GetRequiredService<IQueue>();
            return (Queue) queue;
        }
    }
}