using System;
using System.Diagnostics;
using Coravel.Cli.Commands.Mail.Mailable;
using Coravel.Cli.Commands.Mail.View;

namespace Coravel.Cli.Commands.Mail.Install;

/// <summary>
/// Represents a command that installs the Coravel mailer package and generates the necessary files for using it.
/// </summary>
public sealed class InstallMailCommand
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    public void Execute()
    {
        // Install the Coravel mailer package
        Process.Start("dotnet", "add package Coravel.Mailer").WaitForExit();

        // Generate the view start file
        new CreateViewStartCommand().Execute();

        // Generate an example mailable class
        new CreateMailableCommand().Execute("Example");

        // Generate an example mail view file
        new CreateMailViewCommand().Execute("Example");

        // Generate the view imports file
        new CreateViewImportsCommand().Execute();

        // Print the results to the console
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine();
        Console.WriteLine("Coravel's mailer is installed!");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Note: Don't forget to register the mailer in your ConfigureServices() method inside Startup.cs ;)");
        Console.ResetColor();
    }
}
