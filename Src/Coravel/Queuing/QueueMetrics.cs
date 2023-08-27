namespace Coravel.Queuing;

/// <summary>
/// Represents a class that holds the metrics of a queue.
/// </summary>
public sealed class QueueMetrics
{
    /// <summary>
    /// The number of running tasks in the queue.
    /// </summary>
    private readonly int _runningCount = 0;

    /// <summary>
    /// The number of waiting tasks in the queue.
    /// </summary>
    private readonly int _waitingCount = 0;

    /// <summary>
    /// Initializes a new instance of the QueueMetrics class with the given counts.
    /// </summary>
    /// <param name="runningCount">The number of running tasks in the queue.</param>
    /// <param name="waitingCount">The number of waiting tasks in the queue.</param>
    public QueueMetrics(int runningCount, int waitingCount)
    {
        _runningCount = runningCount;
        _waitingCount = waitingCount;
    }

    /// <summary>
    /// Gets the number of waiting tasks in the queue.
    /// </summary>
    /// <returns>The number of waiting tasks in the queue.</returns>
    public int WaitingCount()
    {
        return _waitingCount;
    }

    /// <summary>
    /// Gets the number of running tasks in the queue.
    /// </summary>
    /// <returns>The number of running tasks in the queue.</returns>
    public int RunningCount()
    {
        return _runningCount;
    }
}
