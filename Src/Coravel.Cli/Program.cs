using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Cli.Commands;
using Coravel.Cli.Commands.Mail.Install;
using Coravel.Cli.Commands.Mail.Mailable;
using Coravel.Cli.Commands.Mail.View;
using McMaster.Extensions.CommandLineUtils;

namespace Coravel.Cli
{
    class Program
    {
        public static void Main(string[] args)
        {
            var app = new CommandLineApplication{
                Name = "coravel"
            };

            app.HelpOption(inherited: true);

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });

            app.Command("install", config =>
                config.OnExecute(() =>
                    new InstallCoravelCommand().Execute()
                )
            );

            app.Command("mail", config =>
            {
                config.OnExecute(() =>
                {
                    config.ShowHelp();
                    return 1;
                });

                config.Command("install", installConfig =>
                {
                    installConfig.Description = "Scaffold coravel's mailer with a generic setup.";
                    installConfig.OnExecute(() =>
                        new InstallMailCommand().Execute()
                    );
                });

                config.Command("new", newConfig =>
                {
                    newConfig.Description = "Create a new coravel Mailable class.";
                    var mailableName = newConfig.Argument<string>("name", "Name of the Mailable to generate.");
                    newConfig.OnExecute(() =>
                    {
                        string mailable = mailableName.Value ?? "Mailable";
                        new CreateMailableCommand().Execute(mailable);
                        new CreateMailViewCommand().Execute(mailable);
                    });
                });
            });

            try {
                int code = app.Execute(args);
                Environment.Exit(code);
            }
            catch(Exception e) {
                Console.WriteLine("Coravel had some trouble... try again.");
                Console.WriteLine(e.Message);
            }
        }
    }
}
