using System;
using Coravel.Cli.Shared;

namespace Coravel.Cli.Commands.Events;

/// <summary>
/// Represents a command that generates an event and a listener class for the user application.
/// </summary>
public sealed class GenerateEventCommand
{
    private readonly static string EventsPath = $"./Events";
    private readonly static string ListenersPath = $"./Listeners";

    /// <summary>
    /// Executes the command with the given event name and listener name.
    /// </summary>
    /// <param name="eventName">The name of the event class to generate.</param>
    /// <param name="listenerName">The name of the listener class to generate.</param>
    public void Execute(string eventName, string listenerName)
    {
        string appName = UserApp.GetAppName();

        bool eventWasCreated = GenerateEventFile(appName, eventName);
        bool listenerWasCreated = GenerateListenerFile(appName, eventName, listenerName);

        Console.ForegroundColor = ConsoleColor.Green;

        if (!eventWasCreated && !listenerWasCreated)
        {
            Console.WriteLine("Both files already exist! Nothing done.");
        }
        else
        {
            if (eventWasCreated)
            {
                Console.WriteLine($"{EventsPath}/{eventName}.cs generated!");
            }
            if (listenerWasCreated)
            {
                Console.WriteLine($"{ListenersPath}/{listenerName}.cs generated!");
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Note: Remember to register your listeners into the service container and register/subscribe your events and listeners.");
        }
        Console.ResetColor();
    }

    private static bool GenerateListenerFile(string appName, string eventName, string listenerName)
    {
        string content = $@"
using System.Threading.Tasks;
using Coravel.Events.Interfaces;
using {appName}.Events;

namespace {appName}.Listeners
{{
    public class {listenerName} : IListener<{eventName}>
    {{
        public Task HandleAsync({eventName} broadcasted)
        {{
            return Task.CompletedTask;
        }}
    }}
}}";
        return Files.WriteFileIfNotCreatedYet(ListenersPath, listenerName + ".cs", content);
    }

    private static bool GenerateEventFile(string appName, string eventName)
    {
        string eventContent = $@"
using Coravel.Events.Interfaces;

namespace {appName}.Events
{{
    public class {eventName} : IEvent
    {{
        public string Message {{ get; set; }}

        public {eventName}(string message)
        {{
            this.Message = message;
        }}
    }}
}}";

        return Files.WriteFileIfNotCreatedYet(EventsPath, eventName + ".cs", eventContent);
    }
}