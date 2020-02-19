using System.Threading;

namespace Coravel.Invocable
{
    public interface ICancellableInvocable
    {
        CancellationToken CancellationToken { get; set; }
    }
}
