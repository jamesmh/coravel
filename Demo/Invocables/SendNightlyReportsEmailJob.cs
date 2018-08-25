using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Invocable;

namespace Demo.Invocables
{
    public class SendNightlyReportsEmailJob : IInvocable
    {
        public async Task Invoke()
        {
            Console.WriteLine($"From SendNightlyReportsJob  on thread {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(65000);
        }
    }
}