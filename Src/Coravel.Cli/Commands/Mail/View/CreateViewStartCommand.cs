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
                .AppendLine("    Layout = \"~/Areas/Coravel/Pages/Mail/Template.cshtml\"")
                .AppendLine("}")
                .ToString();

            Files.WriteFileIfNotCreatedYet(MailViewPath, "_ViewStart.cshtml", viewStartContent);
        }
    }
}