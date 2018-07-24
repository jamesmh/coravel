using System;

namespace Coravel.Cli.Commands
{
    public struct HelpCommand
    {
        public void ProcessCommand(string[] options)
        {
            Console.Write(HelpText);
        }

        private static readonly string HelpText = 
            @"Coravel cli let's you use coravel to build your app super quick!
            Options are:
            
            - install (quickly install coravel)
            - mail install (scafflold a generic mailer setup)
            - mail new (create a new coravel mailable)";
    }
}