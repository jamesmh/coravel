using System;
using System.Diagnostics;

namespace Coravel.Cli.Commands
{
    public struct Install
    {
        public void Execute()
        {
             Console.WriteLine("Please wait while I think for a few seconds...");
            Process.Start("dotnet", "add package coravel").WaitForExit();
            Console.WriteLine("Coravel was installed! Please visit https://github.com/jamesmh/coravel to get started!");
            Console.WriteLine("Don't forget to register Coravel's services in the ConfigureServices() method in Startup.cs!");
        }
    }
}