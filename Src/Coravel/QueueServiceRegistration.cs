using System;
using Coravel.Events.Interfaces;
using Coravel.Queuing;
using Coravel.Queuing.HostedService;
using Coravel.Queuing.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel;

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
        services.AddSingleton<IQueue>(queue =>
            new Queue(
                queue.GetRequiredService<IServiceScopeFactory>(),
                queue.GetService<IDispatcher>() ?? throw new ArgumentNullException(nameof(queue))
            )
        );
        services.AddHostedService<QueuingHost>();
        return services;
    }

    /// <summary>
    /// Configures the queuing services using the service provider.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <returns>A queue configuration object.</returns>
    public static IQueueConfiguration ConfigureQueue(this IServiceProvider provider)
    {
        var queue = provider.GetRequiredService<IQueue>();
        return (Queue)queue;
    }
}