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
        private static Dictionary<Type, List<Type>> eventTypes;

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

        public static IServiceCollection AddEventsFromAssembly<T>(this IServiceCollection services)
        {
            return services.AddEventsFromAssembly(typeof(T).Assembly);
        }

        public static IServiceCollection AddEventsFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                services.AddEventsFromAssembly(assembly);
            }

            return services;
        }

        public static IServiceCollection AddEventsFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            if(eventTypes == null)
            {
                eventTypes = new Dictionary<Type, List<Type>>();
                services.AddEvents();
            }

            foreach (var type in assembly.GetTypes())
            {
                var listenerInterfaces = type
                    .GetInterfaces()
                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IListener<>));

                foreach (var listenerInterface in listenerInterfaces)
                {
                    // IListener always has a single generic argument
                    var eventTypeArgument = listenerInterface.GetGenericArguments().First();

                    if (!eventTypes.TryGetValue(eventTypeArgument, out var listeners))
                    {
                        services.AddTransient(typeof(IEvent), eventTypeArgument);
                        listeners = new List<Type>();
                        eventTypes.Add(eventTypeArgument, listeners);
                    }

                    listeners.Add(type);
                    services.AddTransient(type);
                }
            }

            return services;
        }

        public static IServiceProvider UseCoravelEvents(this IServiceProvider provider)
        {
            var dispatcher = provider.GetRequiredService<IDispatcher>();
            if (!(dispatcher is Dispatcher))
            {
                throw new Exception("Coravel.ConfigureEvents has been mis-configured.");
            }

            if (eventTypes == null)
            {
                throw new Exception("Coravel.AddEventsFromAssembly has not been called.");
            }

            foreach (var eventListenerPair in eventTypes)
            {
                ((Dispatcher)dispatcher).RegisterListenersForEvent(eventListenerPair.Key, eventListenerPair.Value);
            }

            return provider;
        }
    }
}