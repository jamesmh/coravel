using System;
using System.Collections.Concurrent;
using Coravel.Invocable.Interfaces;

namespace Coravel.Invocable
{
    /// <summary>
    /// Implementation of global configuration service for Coravel that manages invocable-level settings
    /// that apply across both scheduler and queue.
    /// </summary>
    public class CoravelGlobalConfiguration : ICoravelGlobalConfiguration
    {
        private readonly ConcurrentDictionary<Type, string> _preventOverlappingTypes = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Registers an invocable type for global prevent overlapping functionality.
        /// </summary>
        /// <typeparam name="TInvocable">The invocable type to register</typeparam>
        /// <param name="uniqueIdentifier">The unique identifier for this prevent overlapping configuration</param>
        public void RegisterPreventOverlapping<TInvocable>(string uniqueIdentifier) where TInvocable : IInvocable
        {
            if (string.IsNullOrWhiteSpace(uniqueIdentifier))
            {
                throw new ArgumentException("Unique identifier cannot be null or whitespace", nameof(uniqueIdentifier));
            }

            _preventOverlappingTypes.AddOrUpdate(typeof(TInvocable), uniqueIdentifier, (key, oldValue) => uniqueIdentifier);
        }

        /// <summary>
        /// Checks if an invocable type has been configured for global prevent overlapping.
        /// </summary>
        /// <typeparam name="TInvocable">The invocable type to check</typeparam>
        /// <param name="uniqueIdentifier">The unique identifier that will be returned if configured</param>
        /// <returns>True if the invocable type is configured for prevent overlapping, false otherwise</returns>
        public bool TryGetPreventOverlapping<TInvocable>(out string uniqueIdentifier) where TInvocable : IInvocable
        {
            return _preventOverlappingTypes.TryGetValue(typeof(TInvocable), out uniqueIdentifier);
        }
    }
}