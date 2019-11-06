using System.Threading;

namespace Coravel.Queuing.Interfaces
{
    public interface ICancellableTask
    {
        CancellationToken Token { get; set; }
    }
}