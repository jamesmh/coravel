using System;
using System.Text;
using Coravel.Cli.Shared;

namespace Coravel.Cli.Commands.Mail.View
{
    public class CreateViewImportsCommand
    {
        private static readonly string MailDirectory = "./Views/Mail";

        public void Execute()
        {
            var appName = UserApp.GetAppName();

            var content = new StringBuilder()
                .AppendLine($"@using {appName}")
                .AppendLine("@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers")
                .AppendLine("@addTagHelper *, Coravel.Mailer.ViewComponents")
                .ToString();

            var wasGenerated = Files.WriteFileIfNotCreatedYet(MailDirectory, "_ViewImports.cshtml", content);

            Console.ForegroundColor = ConsoleColor.Green;
            if (wasGenerated)
            {
                Console.WriteLine($"{MailDirectory}/_ViewImports.cshtml generated!");
            }
            else
            {
                Console.WriteLine($"{MailDirectory}/_ViewImports.cshtml already exists. Nothing done.");
            }
            Console.ResetColor();
        }
    }
}