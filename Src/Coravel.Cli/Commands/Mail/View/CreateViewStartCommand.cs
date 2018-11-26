using System;
using System.Text;
using Coravel.Cli.Shared;

namespace Coravel.Cli.Commands.Mail.View
{
    public class CreateViewStartCommand
    {
        private readonly static string MailViewPath = "./Views/Mail";
        public void Execute()
        {
            string viewStartContent = new StringBuilder()
                .AppendLine("@{")
                .AppendLine("    Layout = \"~/Areas/Coravel/Pages/Mail/Template.cshtml\";")
                .AppendLine("}")
                .ToString();

            bool wasGenerated = Files.WriteFileIfNotCreatedYet(MailViewPath, "_ViewStart.cshtml", viewStartContent);

            Console.ForegroundColor = ConsoleColor.Green;
            if (wasGenerated)
            {
                Console.WriteLine($"{MailViewPath}/_ViewStart.cshtml generated!");
            }
            else
            {
                Console.WriteLine($"{MailViewPath}/_ViewStart.cshtml already exists. Nothing done.");
            }
            Console.ResetColor();
        }
    }
}