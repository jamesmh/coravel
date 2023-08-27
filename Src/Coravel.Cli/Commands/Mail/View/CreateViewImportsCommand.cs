using System;
using System.Text;
using Coravel.Cli.Shared;

namespace Coravel.Cli.Commands.Mail.View;

/// <summary>
/// Represents a command that generates a view imports file for the user application.
/// </summary>
public class CreateViewImportsCommand
{
    /// <summary>
    /// The path where the view imports file is stored.
    /// </summary>
    private static readonly string MailDirectory = "./Views/Mail";

    /// <summary>
    /// Executes the command.
    /// </summary>
    public void Execute()
    {
        // Get the name of the user application
        string appName = UserApp.GetAppName();

        // Generate the content of the view imports file
        string content = new StringBuilder()
            .AppendLine($"@using {appName}")
            .AppendLine("@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers")
            .AppendLine("@addTagHelper *, Coravel.Mailer.ViewComponents")
            .ToString();

        // Write the file if it does not exist yet
        bool wasGenerated = Files.WriteFileIfNotCreatedYet(MailDirectory, "_ViewImports.cshtml", content);

        // Print the results to the console
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
