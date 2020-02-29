using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coravel.Events;
using Coravel.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel
{
    public static class EventServiceRegistration
    {
        public static IServiceCollection AddEvents(this IServiceCollection services)
        {
            services.AddSingleton<IDispatcher>(p =>
                new Dispatcher(p.GetRequiredService<IServiceScopeFactory>()));
            return services;
        }

        public static IEventRegistration ConfigureEvents(this IServiceProvider provider)
        {
            var dispatcher = provider.GetRequiredService<IDispatcher>();
            if (dispatcher is Dispatcher unboxed)
            {
                return unboxed;
            }
            throw new Exception("Coravel.ConfigureEvents has been mis-configured.");
        }


        /// <summary>
        /// Registers all Coravel events and listers from the assembly containing the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventsFromAssembly<T>(this IServiceCollection services)
        {
            return services.AddEventsFromAssembly(typeof(T).Assembly);
        }

        /// <summary>
        /// Registers all Coravel events and listeners from the given assembly.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventsFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            var assemblyScanner = new AssemblyScanner(services);
            services.AddSingleton(assemblyScanner);

            assemblyScanner.AddEventsFromAssembly(assembly);

            return services;
        }

        /// <summary>
        /// Registers all Coravel events and listeners from the given assemblies.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventsFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
        {
            var assemblyScanner = new AssemblyScanner(services);
            services.AddSingleton(assemblyScanner);

            foreach (var assembly in assemblies)
            {
                assemblyScanner.AddEventsFromAssembly(assembly);
            }

            return services;
        }

        /// <summary>
        /// Subscribes all listeners to their corresponding events.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IServiceProvider UseCoravelEvents(this IServiceProvider provider)
        {
            var dispatcher = provider.GetRequiredService<IDispatcher>();
            if (!(dispatcher is Dispatcher))
            {
                throw new Exception("Coravel.ConfigureEvents has been mis-configured.");
            }

            var assemblyScanner = provider.GetService<AssemblyScanner>();
            if (assemblyScanner == null)
            {
                throw new Exception("Coravel.AddEventsFromAssembly has not been called.");
            }

            foreach (var eventListenerPair in assemblyScanner.GetAllEvents())
            {
                ((Dispatcher)dispatcher).RegisterListenersForEvent(eventListenerPair.Key, eventListenerPair.Value);
            }

            return provider;
        }
    }
}