using System;
using System.Text;
using Coravel.Cli.Shared;

namespace Coravel.Cli.Commands.Mail.View;

/// <summary>
/// Represents a command that generates a view start file for the user application.
/// </summary>
public class CreateViewStartCommand
{
    /// <summary>
    /// The path where the view start file is stored.
    /// </summary>
    private readonly static string MailViewPath = "./Views/Mail";

    /// <summary>
    /// Executes the command.
    /// </summary>
    public void Execute()
    {
        // Generate the content of the view start file
        string viewStartContent = new StringBuilder()
            .AppendLine("@{")
            .AppendLine("    Layout = \"~/Areas/Coravel/Pages/Mail/Template.cshtml\";")
            .AppendLine("}")
            .ToString();

        // Write the file if it does not exist yet
        bool wasGenerated = Files.WriteFileIfNotCreatedYet(MailViewPath, "_ViewStart.cshtml", viewStartContent);

        // Print the results to the console
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
