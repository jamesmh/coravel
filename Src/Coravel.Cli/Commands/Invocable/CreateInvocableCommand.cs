using System;
using System.Text;
using Coravel.Cli.Shared;

namespace Coravel.Cli.Commands.Invocable;

/// <summary>
/// Represents a command that generates an invocable class for the user application.
/// </summary>
public class CreateInvocableCommand
{
    private readonly static string InvocablesPath = $"./Invocables";

    /// <summary>
    /// Executes the command with the given invocable name.
    /// </summary>
    /// <param name="invocableName">The name of the invocable class to generate.</param>
    public void Execute(string invocableName)
    {
        string appName = UserApp.GetAppName();

        string content = new StringBuilder()
            .AppendLine("using Coravel.Invocable;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine()
            .AppendLine($"namespace {appName}.Invocables")
            .AppendLine("{")
            .AppendLine($"    public class {invocableName} : IInvocable")
            .AppendLine("    {")
            .AppendLine($"        public {invocableName}()")
            .AppendLine("        {")
            .AppendLine("        }")
            .AppendLine()
            .AppendLine("        public Task Invoke()")
            .AppendLine("        {")
            .AppendLine("            // What is your invocable going to do?")
            .AppendLine("        }")
            .AppendLine("    }")
            .AppendLine("}")
            .ToString();

        bool wasGenerated = Files.WriteFileIfNotCreatedYet(InvocablesPath, invocableName + ".cs", content);

        Console.ForegroundColor = ConsoleColor.Green;

        if (wasGenerated)
        {
            Console.WriteLine($"{InvocablesPath}/{invocableName}.cs generated!");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Note: Don't forget to register your invocable into the service container.");
        }
        else
        {
            Console.WriteLine($"{InvocablesPath}/{invocableName}.cs already exists. Nothing done.");
        }

        Console.ResetColor();
    }
}