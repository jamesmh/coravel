using System;
using System.IO;
using System.Text;

namespace Coravel.Cli.Commands
{
    public class InstallMailCommand
    {
        private static readonly string MailDirectory = "./Views/Mail";
        private static readonly string ViewStartFileName = "_ViewStart.cshtml";
        private static readonly string ExampleViewFileName = "Example.cshtml";
        public void Execute()
        {
            StringBuilder builder = new StringBuilder();

            CreateViewStartFile(builder);
            builder.Clear();

            CreateDummyTemplate(builder);
            builder.Clear();

            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------------------------------------------");
            Console.WriteLine("Check out ~/Views/Mail for the files we generated for you!");
            Console.WriteLine("Don't forget to register the mailer in your ConfigureServices() method inside Startup.cs ;)");
        }

        private void CreateDummyTemplate(StringBuilder builder)
        {
            string exampleContent = builder
                .AppendLine("@* To use inside a mailable: this.View(\"~/Views/Mail/Example.cshtml\", null) *@")
                .AppendLine()
                .AppendLine("@{")
                .AppendLine("   ViewBag.Heading = \"Welcome!\";")
                .AppendLine("   ViewBag.Preview = \"Example Email\";")
                .AppendLine("}")
                .AppendLine()
                .AppendLine("<p>")
                .AppendLine("   This is an example email to get you started!")
                .AppendLine("   To render a button inside your email, use the EmailLinkButton component:")
                .AppendLine("   @await Component.InvokeAsync(\"EmailLinkButton\", new  { text = \"Click me\", url = \"www.google.com\" })")
                .AppendLine("</p>")
                .AppendLine()
                .AppendLine("@section links")
                .AppendLine("{")
                .AppendLine("   <a href=\"https://www.google.com\">Google</a> | <a href=\"https://www.google.com\">Google</a>")
                .AppendLine("}")
                .ToString();

            WriteFileIfNotCreatedYet(MailDirectory, ExampleViewFileName, exampleContent);
        }

        private static void CreateViewStartFile(StringBuilder builder)
        {
            string viewStartContent = builder
                    .AppendLine("@{")
                    .AppendLine("    Layout = \"~/Areas/Coravel/Pages/Mail/Template.cshtml\"")
                    .AppendLine("}")
                    .ToString();

            WriteFileIfNotCreatedYet(MailDirectory, ViewStartFileName, viewStartContent);
        }

        private static void WriteFileIfNotCreatedYet(string path, string filename, string content)
        {
            string fullFilePath = path + "/" + filename;

            Directory.CreateDirectory(path);

            if (!File.Exists(fullFilePath))
            {                
                using (var file = File.CreateText(fullFilePath))
                {
                    file.Write(content);
                }
            }
        }
    }
}