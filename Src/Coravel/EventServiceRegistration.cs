using System;
using Coravel.Events;
using Coravel.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel;

/// <summary>
/// Provides extension methods for registering and configuring event services.
/// </summary>
public static partial class EventServiceRegistration
{
    /// <summary>
    /// Adds the event services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddEvents(this IServiceCollection services)
    {
        services.AddSingleton<IDispatcher>(p =>
            new Dispatcher(p.GetRequiredService<IServiceScopeFactory>()));

        return services;
    }

    /// <summary>
    /// Configures the event services using the service provider.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <returns>An event registration object.</returns>
    public static IEventRegistration ConfigureEvents(this IServiceProvider provider)
    {
        var dispatcher = provider.GetRequiredService<IDispatcher>();

        if (dispatcher is Dispatcher unboxed)
        {
            return unboxed;
        }

        throw new EventServiceRegistrationConfigureEventsException("Coravel.ConfigureEvents has been mis-configured.");
    }
}
