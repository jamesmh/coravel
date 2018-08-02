using System.Text;
using Coravel.Cli.Shared;

namespace Coravel.Cli.Commands.Mail.View
{
    public class CreateViewImportsCommand
    {
        private static readonly string MailDirectory = "./Views/Mail";

        public void Execute()
        {
            string appName = UserApp.GetAppName();

            string content = new StringBuilder()
                .AppendLine($"@using {appName}")
                .AppendLine("@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers")
                .AppendLine("@addTagHelper *, Coravel.Razor.ViewComponents")
                .ToString();

            Files.WriteFileIfNotCreatedYet(MailDirectory, "_ViewImports.cshtml", content);
        }
    }
}