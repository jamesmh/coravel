using System.Threading.Tasks;

namespace Coravel.Scheduling.Schedule.Interfaces
{
    public interface IInvocable
    {
        Task Invoke();
    }
}