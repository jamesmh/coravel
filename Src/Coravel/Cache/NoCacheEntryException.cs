
namespace Coravel.Cache;

/// <summary>
/// Represents an exception that is thrown when a cache entry is not found.
/// </summary>
public class NoCacheEntryException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the NoCacheEntryException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NoCacheEntryException(string message) : base(message)
    {

    }
}
