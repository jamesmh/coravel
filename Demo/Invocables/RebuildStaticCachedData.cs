using Coravel.Invocable;
using System.Threading.Tasks;

namespace Demo.Invocables
{
    public class RebuildStaticCachedData : IInvocable
    {
        public RebuildStaticCachedData()
        {
        }

        public Task Invoke()
        {
            return Task.CompletedTask;
        }
    }
}
