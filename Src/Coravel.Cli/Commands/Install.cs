using System;
using System.Diagnostics;

namespace Coravel.Cli.Commands
{
    public struct Install
    {
        public void Execute()
        {
            Process.Start("dotnet", "add package coravel").WaitForExit();
            Console.WriteLine(FinishedText);
        }

        private static readonly string FinishedText = 
            @"Coravel was installed! Please visit https://github.com/jamesmh/coravel to get started!";
    }
}