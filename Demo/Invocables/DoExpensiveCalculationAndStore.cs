using System.Threading.Tasks;
using Coravel.Invocable;

namespace Demo.Invocables
{
    public class DoExpensiveCalculationAndStore : IInvocable
    {
        public Task Invoke()
        {
            throw new System.NotImplementedException();
        }
    }
}