using System;
using System.Linq;
using Coravel.Events.Interfaces;
using Coravel.Queuing;
using Coravel.Queuing.HostedService;
using Coravel.Queuing.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                new Queue(
                    p.GetRequiredService<IServiceScopeFactory>(),
                    p.GetService<IDispatcher>()
                )
            );
            services.AddHostedService<QueuingHost>();
            return services;
        }

        public static IServiceCollection AddQueue(this IServiceCollection services, string queueName)
        {
            services.AddSingleton<IQueue>(p =>
                new Queue(
                    queueName,
                    p.GetRequiredService<IServiceScopeFactory>(),
                    p.GetService<IDispatcher>()
                )
            );

            //It is not possible to use services.AddHostedService here.
            //AddHostedService method only registers the service if it is not one already registered.
            //Please have a look at:
            //https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/Microsoft.Extensions.Hosting.Abstractions/src/ServiceCollectionHostedServiceExtensions.cs
            services.AddSingleton<IHostedService, QueuingHost>(p =>
            {
                var queue = p.GetServices<IQueue>().First(x => x.QueueName == queueName);
                var configuration = p.GetRequiredService<IConfiguration>();
                var logger = p.GetRequiredService<ILogger<QueuingHost>>();
                return new QueuingHost(queue, configuration, logger);
            });
            return services;
        }

        public static IServiceCollection AddQueues(this IServiceCollection services, Action<IServiceCollection> registerQueues)
        {
            registerQueues(services);
            services.AddSingleton<IQueues, QueuesCollection>();
            return services;
        }

        public static IQueueConfiguration ConfigureQueue(this IServiceProvider provider)
        {
            var queue = provider.GetRequiredService<IQueue>();
            return (Queue) queue;
        }
    }
}