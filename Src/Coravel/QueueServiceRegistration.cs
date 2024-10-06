using System;
using Coravel.Events.Interfaces;
using Coravel.Queuing;
using Coravel.Queuing.HostedService;
using Coravel.Queuing.Interfaces;
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
            services.AddSingleton<QueueOptions>(new QueueOptions());
            services.AddSingleton<IQueue>(p =>
                new Queue(
                    p.GetRequiredService<IServiceScopeFactory>(),
                    p.GetService<IDispatcher>()
                )
            );
            services.AddHostedService<QueuingHost>();
            return services;
        }

        public static IServiceCollection AddQueue(this IServiceCollection services, Action<QueueOptions> options)
        {
            var opt = new QueueOptions();
            options(opt);

            services.AddSingleton<QueueOptions>(opt);
            services.AddSingleton<IQueue>(p =>
                new Queue(
                    p.GetRequiredService<IServiceScopeFactory>(),
                    p.GetService<IDispatcher>()
                )
            );
            services.AddHostedService<QueuingHost>();
            return services;
        }

        public static IQueueConfiguration ConfigureQueue(this IServiceProvider provider)
        {
            var queue = provider.GetRequiredService<IQueue>();
            return (Queue) queue;
        }
    }
}