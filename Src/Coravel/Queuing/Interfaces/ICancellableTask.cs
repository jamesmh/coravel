using System.Threading;

namespace Coravel.Queuing.Interfaces;

/// <summary>
/// Defines a contract for a task that can be cancelled.
/// </summary>
public interface ICancellableTask
{
    /// <summary>
    /// Gets or sets the cancellation token for the task.
    /// </summary>
    CancellationToken Token { get; set; }
}
