using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Coravel.Cli.Commands.Mail.Mailable;
using Coravel.Cli.Commands.Mail.View;
using Coravel.Cli.Shared;

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

            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------------------------------------------");
            Console.WriteLine("Check out ~/Views/Mail - the following was generated: ");
            Console.WriteLine("    _ViewStart.cshtml    - Configures mail views to use Coravel's e-mail templates");
            Console.WriteLine("    _ViewImports.cshtml  - Allows you use Coravel's view components");
            Console.WriteLine("    Example.cshtml`      - A sample mail view");
            Console.WriteLine();
            Console.WriteLine("Coravel also created a new ~/Mailables folder with a sample Mailable for you!");
            Console.WriteLine("Don't forget to register the mailer in your ConfigureServices() method inside Startup.cs ;)");
        }
    }
}