using System.IO;
using System.Linq;

namespace Coravel.Cli.Shared;

/// <summary>
/// Represents a class that provides information about the user application.
/// </summary>
public sealed class UserApp
{
    private UserApp() { }

    /// <summary>
    /// Gets the name of the user application from the current directory.
    /// </summary>
    /// <returns>The name of the user application.</returns>
    public static string GetAppName() =>
        // Replace the slashes with backslashes
        Directory.GetCurrentDirectory().Replace("/", "\\")
        // Split the path by backslashes
        .Split('\\')
        // Get the last element of the split path
        .Last();
}
