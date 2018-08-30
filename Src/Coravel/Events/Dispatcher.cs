using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel.Events
{
    /// <summary>
    /// Will dispatch Coravel events and broadcast them to the appropriate listeners.
    ///  </summary>
    public class Dispatcher : IDispatcher, IEventRegistration
    {
        private IServiceScopeFactory _scopeFactory;
        private Dictionary<Type, List<Type>> _events;

        public Dispatcher(IServiceScopeFactory scopeFactory)
        {
            this._scopeFactory = scopeFactory;
            this._events = new Dictionary<Type, List<Type>>();
        }

        /// <summary>
        /// Register an event. You may subscribe listeners to this event by further chaining.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public IEventSubscription<TEvent> Register<TEvent>() where TEvent : IEvent
        {
            var listeners = new List<Type>();
            this._events.TryAdd(typeof(TEvent), listeners);
            return new EventSubscription<TEvent>(listeners);
        }

        /// <summary>
        ///  Broadcasts an event to be handled by it's subscribed listeners.
        /// </summary>
        /// <param name="toBroadcast"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public async Task Broadcast<TEvent>(TEvent toBroadcast) where TEvent : IEvent
        {
            if (this._events.TryGetValue(toBroadcast.GetType(), out var listeners))
            {
                foreach (var listenerType in listeners)
                {
                    using (var scope = this._scopeFactory.CreateScope())
                    {
                        if (scope.ServiceProvider.GetService(listenerType) is IListener<TEvent> listener)
                        {
                            var keepBroacasting = await listener.HandleAsync(toBroadcast);

                            if (!keepBroacasting)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Represents an event subscription.
    /// </summary>
    public struct EventSubscription<TEvent> : IEventSubscription<TEvent> where TEvent : IEvent
    {
        private List<Type> _listeners;

        public EventSubscription(List<Type> listeners)
        {
            this._listeners = listeners;
        }

        /// <summary>
        /// Subscribe a listener to an event.
        /// </summary>
        /// <typeparam name="TListener"></typeparam>
        /// <returns></returns>
        public IEventSubscription<TEvent> Subscribe<TListener>() where TListener : IListener<TEvent>
        {
            Type listenerType = typeof(TListener);
            bool listenerAlreadyRegistered = this._listeners.Any(t => t.Equals(listenerType));

            if (!listenerAlreadyRegistered)
            {
                this._listeners.Add(typeof(TListener));
            }

            return this;
        }
    }
}