using System;
using System.Threading.Tasks;
using Coravel.Invocable;

namespace DotNet6
{
    public class SampleInvocable : IInvocable
    {
        public Task Invoke()
        {
            Console.WriteLine("Sample Invocable ran!");
            return Task.CompletedTask;
        }
    }
}