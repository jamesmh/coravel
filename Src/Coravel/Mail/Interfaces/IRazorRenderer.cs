using System.Threading.Tasks;

namespace Coravel.Mail.Interfaces
{
    public interface IRazorRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}