using System;
using System.Text;
using Coravel.Cli.Shared;

namespace Coravel.Cli.Commands.Mail.View;

/// <summary>
/// Represents a command that generates a mail view file for the user application.
/// </summary>
public sealed class CreateMailViewCommand
{
    private static readonly string MailDirectory = "./Views/Mail";

    /// <summary>
    /// Executes the command with the given file name.
    /// </summary>
    /// <param name="fileName">The name of the mail view file to generate.</param>
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

        bool wasGenerated = Files.WriteFileIfNotCreatedYet(MailDirectory, fileName + ".cshtml", content);

        Console.ForegroundColor = ConsoleColor.Green;

        if (wasGenerated)
        {
            Console.WriteLine($"{MailDirectory}/{fileName}.cshtml generated!");
        }
        else
        {
            Console.WriteLine($"{MailDirectory}/{fileName}.cshtml already exists. Nothing done.");
        }

        Console.ResetColor();
    }
}