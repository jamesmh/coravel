using System;
using System.Diagnostics;

namespace Coravel.Cli.Commands;

/// <summary>
/// Represents a command that installs the Coravel package for the user application.
/// </summary>
public struct InstallCoravelCommand
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    public void Execute()
    {
        // Start a process to add the Coravel package
        Process.Start("dotnet", "add package coravel").WaitForExit();
        Console.WriteLine("");
        // Print the results to the console
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("--------------------------------------------------------------------------------------------");
        Console.WriteLine("Coravel was installed! Please visit https://github.com/jamesmh/coravel to get started!");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Note: Don't forget to register Coravel's services in the ConfigureServices() method in Startup.cs!");
        Console.ResetColor();
    }
}
