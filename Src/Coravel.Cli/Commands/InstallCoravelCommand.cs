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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("--------------------------------------------------------------------------------------------");
            Console.WriteLine("Coravel was installed! Please visit https://github.com/jamesmh/coravel to get started!");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Note: Don't forget to register Coravel's services in the ConfigureServices() method in Startup.cs!");
            Console.ResetColor();
        }
    }
}