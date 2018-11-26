using System.Threading.Tasks;
using Coravel.Invocable;
using Demo.Models;

namespace Demo.Invocables
{
    public class SendFormToExternalApi : IInvocable
    {
        public SendFormToExternalApi WithForm(SampleForm form) {
            return this;
        }

        public Task Invoke()
        {
            throw new System.NotImplementedException();
        }
    }
}