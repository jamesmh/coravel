using System.Text;
using Coravel.Cli.Shared;

namespace Coravel.Cli.Commands.Mail.View
{
    public class CreateMailViewCommand
    {
        private static readonly string MailDirectory = "./Views/Mail";

        public void Execute(string fileName)
        {
            string content = new StringBuilder()
                .AppendLine("@{")
                .AppendLine("   ViewBag.Heading = \"Welcome!\";")
                .AppendLine("   ViewBag.Preview = \"Example Email\";")
                .AppendLine("}")
                .AppendLine()
                .AppendLine("<p>")
                .AppendLine("   Let's see what you can build!")
                .AppendLine("   To render a button inside your email, use the EmailLinkButton component:")
                .AppendLine("   @await Component.InvokeAsync(\"EmailLinkButton\", new  { text = \"Click Me!\", url = \"https://www.google.com\" })")
                .AppendLine("</p>")
                .AppendLine()
                .AppendLine("@section links")
                .AppendLine("{")
                .AppendLine("   <a href=\"https://github.com/jamesmh/coravel\">Coravel</a>")
                .AppendLine("}")
                .ToString();

            Files.WriteFileIfNotCreatedYet(MailDirectory, fileName + ".cshtml", content);
        }
    }
}