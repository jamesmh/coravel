using System.Threading.Tasks;

namespace Coravel.Invocable
{
    public interface IInvocable
    {
        Task Invoke();
    }
}