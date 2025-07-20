using System;
using Coravel.Invocable;
using Coravel.Invocable.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel
{
    /// <summary>
    /// IServiceProvider extensions for configuring global Coravel invocable settings.
    /// </summary>
    public static class CoravelGlobalConfigurationServiceRegistration
    {
        /// <summary>
        /// Configures the specified invocable type to prevent overlapping executions globally.
        /// This will prevent overlapping for both scheduled tasks and queued tasks.
        /// </summary>
        /// <typeparam name="TInvocable">The invocable type to configure</typeparam>
        /// <param name="provider">The service provider</param>
        /// <param name="uniqueIdentifier">A unique identifier for this invocable's prevent overlapping configuration</param>
        /// <returns>The service provider for chaining</returns>
        public static IServiceProvider PreventOverlapping<TInvocable>(this IServiceProvider provider, string uniqueIdentifier) 
            where TInvocable : IInvocable
        {
            var globalConfiguration = provider.GetRequiredService<ICoravelGlobalConfiguration>();
            globalConfiguration.RegisterPreventOverlapping<TInvocable>(uniqueIdentifier);
            return provider;
        }

        /// <summary>
        /// Registers the global configuration service. This is called automatically when using AddScheduler or AddQueue.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        internal static IServiceCollection AddCoravelGlobalConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<ICoravelGlobalConfiguration, CoravelGlobalConfiguration>();
            return services;
        }
    }
}