using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Cli.Commands;
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
                    new Install().Execute()
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
                        new CreateMailCommand().Execute(mailableName.Value ?? "NewMailable")
                    );
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
