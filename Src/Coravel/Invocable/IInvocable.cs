using System.Threading.Tasks;

namespace Coravel.Invocable;

/// <summary>
/// Represents some action in your system that can be invoked by various
/// parts of Coravel. 
/// 
/// For example, you may schedule or queue an invocable class
/// to be instantiated by the service container when due or dequeued.
/// </summary>
public interface IInvocable
{
    /// <summary>
    /// Execute the logic/code for this invocable instance.
    /// </summary>
    /// <returns></returns>
    Task Invoke();
}