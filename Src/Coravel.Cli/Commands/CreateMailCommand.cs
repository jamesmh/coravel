using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Coravel.Cli.Commands
{
    public struct CreateMailCommand
    {
        public void Execute(string mailableName)
        {
            string path = $"./Mailables";
            string fileName = mailableName + ".cs";

            if (File.Exists(path))
            {
                FileExists(path);
                return;
            }
            else
            {
                WriteNewFile(mailableName, path, fileName);
                Console.WriteLine($"A new mailable was created at {path}!");
            }
        }

        private static void WriteNewFile(string mailableName, string path, string fileName)
        {
            string fullFilePath = path + "/" + fileName;

            Directory.CreateDirectory(path);          

            using (var writer = File.CreateText(fullFilePath))
            {                
                string appName = GetAppName();
                StringBuilder builder = new StringBuilder();

                builder
                    .AppendLine("using Coravel.Mail;")
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
                    .AppendLine($"                .View(\"~/Views/Mail/{mailableName}.cshtml\", null); // This view was not created, but is a suggested location :)")
                    .AppendLine("        }")
                    .AppendLine("    }")
                    .AppendLine("}");

                writer.Write(builder.ToString());
            }
        }

        private static string GetAppName() =>
            Directory.GetCurrentDirectory().Split('\\').Last();

        private void FileExists(string filePath)
        {
            Console.WriteLine("$File already exists at {filePath}.");
        }
    }
}