using System;
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
            new CreateViewStartCommand().Execute();
            new CreateMailableCommand().Execute("Example");
            new CreateMailViewCommand().Execute("Example");
            new CreateViewStartCommand().Execute();

            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------------------------------------------");
            Console.WriteLine("Check out ~/Views/Mail - we created: ");
            Console.WriteLine("    _ViewStart.cshtml: Registers Coravel's e-mail templates as the default layout for your mail views.");
            Console.WriteLine("    Example.cshtml: A sample mail view.");
            Console.WriteLine();
            Console.WriteLine("Coravel also created a new ~/Mailables folder with a sample Mailable for you!");

            Console.WriteLine("Don't forget to register the mailer in your ConfigureServices() method inside Startup.cs ;)");
        }
    }
}