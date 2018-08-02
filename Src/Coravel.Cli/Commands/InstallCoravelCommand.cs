using System;
using System.Diagnostics;

namespace Coravel.Cli.Commands
{
    public struct InstallCoravelCommand
    {
        public void Execute()
        {
            Process.Start("dotnet", "add package coravel").WaitForExit();
            Console.WriteLine("");
            Console.WriteLine("--------------------------------------------------------------------------------------------");
            Console.WriteLine("Coravel was installed! Please visit https://github.com/jamesmh/coravel to get started!");
            Console.WriteLine("Don't forget to register Coravel's services in the ConfigureServices() method in Startup.cs!");
        }
    }
}