using System;
using System.Threading.Tasks;
using Coravel.Invocable;

namespace Demo.Invocables
{
    public class DoExpensiveCalculationAndStore : IInvocable
    {
        public async Task Invoke()
        {
            Console.Write("Doing expensive calculation for 15 sec...");
            await Task.Delay(15000);
            Console.Write("Expensive calculation done.");
        }
    }
}