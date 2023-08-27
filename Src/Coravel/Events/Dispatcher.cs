using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel.Events;

/// <summary>
/// Will dispatch Coravel events and broadcast them to the appropriate listeners.
///  </summary>
public sealed class Dispatcher : IDispatcher, IEventRegistration
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<Type, List<Type>> _events;

    public Dispatcher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _events = new Dictionary<Type, List<Type>>();
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

        if (_events.ContainsKey(eventType))
        {
            listeners = _events[eventType];
        }
        else
        {
            _events.Add(eventType, listeners);
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
        if (_events.TryGetValue(toBroadcast.GetType(), out var listeners))
        {
            foreach (var listenerType in listeners)
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var obj = scope.ServiceProvider.GetService(listenerType);
                if (obj is IListener<TEvent> listener)
                {
                    await listener.HandleAsync(toBroadcast);
                }
                else
                {
                    // Depending on what assemblies the events, listeners and calling assmebly are - the cast
                    // above doesn't work (even though the type really does implement the interface).
                    // Not sure why this happens. Might be a side effect of running inside a unit test proj. Dunno.
                    // This condition will catch those cases and default to reflection.                        
                    var result = listenerType.GetMethod("HandleAsync")?.Invoke(obj, new object[] { toBroadcast });

                    if (result is Task task)
                    {
                        await (task);
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
    private readonly List<Type> _listeners;

    public EventSubscription(List<Type> listeners) => _listeners = listeners;

    /// <summary>
    /// Subscribe a listener to an event.
    /// </summary>
    /// <typeparam name="TListener"></typeparam>
    /// <returns></returns>
    public IEventSubscription<TEvent> Subscribe<TListener>() where TListener : IListener<TEvent>
    {
        Type listenerType = typeof(TListener);
        bool listenerAlreadyRegistered = _listeners.Contains(listenerType);

        if (!listenerAlreadyRegistered)
        {
            _listeners.Add(typeof(TListener));
        }

        return this;
    }
}