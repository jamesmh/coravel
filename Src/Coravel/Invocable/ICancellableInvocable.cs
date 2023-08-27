using System.Threading;

namespace Coravel.Invocable;

/// <summary>
/// Defines a contract for an invocable that can be cancelled.
/// </summary>
public interface ICancellableInvocable
{
    /// <summary>
    /// Gets or sets the cancellation token for the invocable.
    /// </summary>
    CancellationToken CancellationToken { get; set; }
}

