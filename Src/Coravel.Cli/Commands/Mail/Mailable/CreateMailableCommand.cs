using System;
using System.Text;
using Coravel.Cli.Shared;

namespace Coravel.Cli.Commands.Mail.Mailable
{
    public class CreateMailableCommand
    {
        private readonly static string MailablePath = $"./Mailables";
        public void Execute(string mailableName)
        {
            string appName = UserApp.GetAppName();

            string content = new StringBuilder()
                .AppendLine("using Coravel.Mailer.Mail;")
                .AppendLine()
                .AppendLine($"namespace {appName}.Mailables")
                .AppendLine("{")
                .AppendLine($"    public class {mailableName} : Mailable<string>")
                .AppendLine("    {")
                .AppendLine($"        public {mailableName}()")
                .AppendLine("        {")
                .AppendLine("            // Inject a model if using this.View()")
                .AppendLine("        }")
                .AppendLine()
                .AppendLine("        public override void Build()")
                .AppendLine("        {")
                .AppendLine("            this.To(\"coravel@is.awesome\")")
                .AppendLine("                .From(\"from@test.com\")")
                .AppendLine($"                .View(\"~/Views/Mail/{mailableName}.cshtml\", null);")
                .AppendLine("        }")
                .AppendLine("    }")
                .AppendLine("}")
                .ToString();

            bool wasGenerated = Files.WriteFileIfNotCreatedYet(MailablePath, mailableName + ".cs", content);

            Console.ForegroundColor = ConsoleColor.Green;
            if (wasGenerated)
            {
                Console.WriteLine($"{MailablePath}/{mailableName}.cs generated!");
            }
            else
            {
                Console.WriteLine($"{MailablePath}/{mailableName}.cs already exists. Nothing done.");
            }
            Console.ResetColor();
        }
    }
}