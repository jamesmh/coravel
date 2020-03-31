namespace Coravel.Invocable
{
    public interface IInvocableWithPayload<T>
    {
        T Payload { get; set; }
    }
}