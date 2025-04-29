using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coravel.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel
{
    internal class AssemblyScanner
    {
        private readonly Dictionary<Type, List<Type>> _eventTypes;
        private readonly IServiceCollection _services;

        public AssemblyScanner(IServiceCollection services)
        {
            this._eventTypes = new Dictionary<Type, List<Type>>();
            this._services = services;
            this._services.AddEvents();
        }

        public void AddEventsFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var listenerInterfaces = type
                    .GetInterfaces()
                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IListener<>));

                foreach (var listenerInterface in listenerInterfaces)
                {
                    // IListener always has a single generic argument
                    var eventTypeArgument = listenerInterface.GetGenericArguments().First();

                    if (!this._eventTypes.TryGetValue(eventTypeArgument, out var listeners))
                    {
                        this._services.AddTransient(typeof(IEvent), eventTypeArgument);
                        listeners = new List<Type>();
                        this._eventTypes.Add(eventTypeArgument, listeners);
                    }

                    listeners.Add(type);
                    this._services.AddTransient(type);
                }
            }
        }

        public Dictionary<Type, List<Type>> GetAllEvents()
        {
            return this._eventTypes;
        }
    }
}
