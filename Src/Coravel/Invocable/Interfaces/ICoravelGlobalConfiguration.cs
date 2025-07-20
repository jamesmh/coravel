using System;

namespace Coravel.Invocable.Interfaces
{
    /// <summary>
    /// Global configuration service for Coravel that manages invocable-level settings
    /// that apply across both scheduler and queue.
    /// </summary>
    public interface ICoravelGlobalConfiguration
    {
        /// <summary>
        /// Registers an invocable type for global prevent overlapping functionality.
        /// </summary>
        /// <typeparam name="TInvocable">The invocable type to register</typeparam>
        /// <param name="uniqueIdentifier">The unique identifier for this prevent overlapping configuration</param>
        void RegisterPreventOverlapping<TInvocable>(string uniqueIdentifier) where TInvocable : IInvocable;

        /// <summary>
        /// Checks if an invocable type has been configured for global prevent overlapping.
        /// </summary>
        /// <typeparam name="TInvocable">The invocable type to check</typeparam>
        /// <param name="uniqueIdentifier">The unique identifier that will be returned if configured</param>
        /// <returns>True if the invocable type is configured for prevent overlapping, false otherwise</returns>
        bool TryGetPreventOverlapping<TInvocable>(out string uniqueIdentifier) where TInvocable : IInvocable;
    }
}