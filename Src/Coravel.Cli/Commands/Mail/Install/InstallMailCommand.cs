using System;
using System.Diagnostics;
using Coravel.Cli.Commands.Mail.Mailable;
using Coravel.Cli.Commands.Mail.View;

namespace Coravel.Cli.Commands.Mail.Install
{
    public class InstallMailCommand
    {
        public void Execute()
        {
            Process.Start("dotnet", "add package Coravel.Mailer").WaitForExit();
            new CreateViewStartCommand().Execute();
            new CreateMailableCommand().Execute("Example");
            new CreateMailViewCommand().Execute("Example");
            new CreateViewImportsCommand().Execute();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine();
            Console.WriteLine("Coravel's mailer is installed!");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Note: Don't forget to register the mailer in your ConfigureServices() method inside Startup.cs ;)");
            Console.ResetColor();
        }
    }
}