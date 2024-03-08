using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var eventType = typeof(TEvent);

            if (this._events.ContainsKey(eventType))
            {
                listeners = this._events[eventType];
            }
            else
            {
                this._events.Add(eventType, listeners);
            }

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
                    await using (var scope = this._scopeFactory.CreateAsyncScope())
                    {
                        var obj = scope.ServiceProvider.GetRequiredService(listenerType);
                        if (obj is IListener<TEvent> listener)
                        {
                            await listener.HandleAsync(toBroadcast);
                        }
                        else {
                            // Depending on what assemblies the events, listeners and calling assmebly are - the cast
                            // above doesn't work (even though the type really does implement the interface).
                            // Not sure why this happens. Might be a side effect of running inside a unit test proj. Dunno.
                            // This condition will catch those cases and default to reflection.
                            //
                            // But! It is possible that Listener class implements more IListener<TEvent>s, so we should directly invoke the anticipated method.
                            // Do not attempt to .GetMethod("HandleAsync") because this might be ambiguous and throw an exception
                            var result = listenerType.InvokeMember(nameof(IListener<TEvent>.HandleAsync), BindingFlags.InvokeMethod,
                                Type.DefaultBinder, obj, new object[] {toBroadcast});
                            await (result as Task);
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