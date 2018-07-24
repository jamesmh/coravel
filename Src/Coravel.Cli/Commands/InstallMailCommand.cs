using System;
using System.IO;
using System.Text;

namespace Coravel.Cli.Commands
{
    public class InstallMailCommand
    {
        private static readonly string ViewStart = "./Views/Mail/_ViewStart.cshtml";
        private static readonly string ExampleView = "./Views/Mail/Example.cshtml";
        public void Execute()
        {
            StringBuilder builder = new StringBuilder();

            CreateViewStartFile(builder);
            builder.Clear();

            CreateDummyTemplate(builder);
            builder.Clear();

            Console.WriteLine("Check out ~/Views/Mail for the files we generated for you!");
            Console.WriteLine("Don't forget to register the mailer in your ConfigureServices() method inside Startup.cs ;)");
        }

        private void CreateDummyTemplate(StringBuilder builder)
        {
            string exampleContent = builder
                .AppendLine("@* If your mailable calls this.View(viewPath, viewModel), you should include an \"@model\" statement here :) *@")
                .AppendLine("@{")
                .AppendLine("   ViewBag.Heading = \"Welcome!\";")
                .AppendLine("   ViewBag.Preview = \"Example Email\";")
                .AppendLine("}")
                .AppendLine("<p>")
                .AppendLine("   This is an example email to get you started!")
                .AppendLine("   To render a button inside your email, use the EmailLinkButton component:")
                .AppendLine("   @await Component.InvokeAsync(\"EmailLinkButton\", new  { text = \"Click me\", url = \"www.google.com\" })")
                .AppendLine("</p>")
                .AppendLine("")
                .AppendLine("@section links")
                .AppendLine("{")
                .AppendLine("   <a href=\"https://www.google.com\">Google</a> | <a href=\"https://www.google.com\">Google</a>")
                .AppendLine("}")
                .ToString();

            WriteFileIfNotCreatedYet(ExampleView, exampleContent);
        }

        private static void CreateViewStartFile(StringBuilder builder)
        {
            string viewStartContent = builder
                    .AppendLine("@{")
                    .AppendLine("    Layout = \"~/Areas/Coravel/Pages/Mail/Template.cshtml\"")
                    .AppendLine("\"}\"")
                    .ToString();

            WriteFileIfNotCreatedYet(ViewStart, viewStartContent);
        }

        private static void WriteFileIfNotCreatedYet(string path, string content)
        {
            if (!File.Exists(path))
            {
                using (var file = File.CreateText(path))
                {
                    file.Write(content);
                }
            }
        }
    }
}