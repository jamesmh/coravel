using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Coravel.Invocable;

namespace WorkerScheduler
{
    public class MyFirstInvocable : IInvocable
    {
        public Task Invoke()
        {
            Console.WriteLine("This is my first invocable!");
            return Task.CompletedTask;
        }
    }
}