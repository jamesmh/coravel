namespace Coravel.Invocable;

/// <summary>
/// Defines a contract for an invocable that has a payload.
/// </summary>
/// <typeparam name="T">The type of the payload.</typeparam>
public interface IInvocableWithPayload<T>
{
    /// <summary>
    /// Gets or sets the payload for the invocable.
    /// </summary>
    T Payload { get; set; }
}
