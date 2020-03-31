namespace Coravel.Invocable
{
    public interface IInvocableWithPayload<T>
    {
        T Parameters { get; set; }
    }
}