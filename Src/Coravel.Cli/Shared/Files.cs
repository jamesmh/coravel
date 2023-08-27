using System.IO;

namespace Coravel.Cli.Shared;

/// <summary>
/// Represents a utility class that handles file operations.
/// </summary>
public sealed class Files
{
    private Files() { }

    /// <summary>
    /// Writes a file with the given content to the specified path and filename, if it does not exist yet.
    /// </summary>
    /// <param name="path">The path where the file should be stored.</param>
    /// <param name="filename">The name of the file to write.</param>
    /// <param name="content">The content of the file to write.</param>
    /// <returns>True if the file was written, false if it already existed.</returns>
    public static bool WriteFileIfNotCreatedYet(string path, string filename, string content)
    {
        // Construct the full file path
        string fullFilePath = $"{path}/{filename}";

        // Create the directory if it does not exist
        Directory.CreateDirectory(path);

        // Check if the file already exists
        if (!File.Exists(fullFilePath))
        {
            // Create and write the file with the content
            using var file = File.CreateText(fullFilePath);
            file.Write(content);

            return true;
        }
        else
        {
            // The file already exists, do nothing
            return false;
        }
    }
}
