using System.Text;
using Coravel.Cli.Shared;

namespace Coravel.Cli.Commands.Invocable
{
    public class CreateInvocableCommand
    {
        private readonly static string InvocablesPath = $"./Invocables";
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

            Files.WriteFileIfNotCreatedYet(InvocablesPath, invocableName + ".cs", content);
        }
    }
}