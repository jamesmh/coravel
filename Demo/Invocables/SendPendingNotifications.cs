using Coravel.Invocable;
using System.Threading.Tasks;

namespace Demo.Invocables
{
    public class SendPendingNotifications : IInvocable
    {
        public SendPendingNotifications()
        {
        }

        public Task Invoke()
        {
            return Task.CompletedTask;
        }
    }
}
