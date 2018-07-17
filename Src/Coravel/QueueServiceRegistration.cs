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
        public static IQueueConfiguration AddQueue(this IServiceCollection services)
        {
            Queue queue = new Queue();
            services.AddSingleton<IQueue>(queue);
            services.AddHostedService<QueuingHost>();

            return queue;
        }
    }
}