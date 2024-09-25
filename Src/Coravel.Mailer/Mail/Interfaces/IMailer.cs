using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coravel.Mailer.Mail.Interfaces
{
    public interface IMailer : ICanSendMail
    {
        Task<string> RenderAsync<T>(Mailable<T> mailable);
        Task SendAsync<T>(Mailable<T> mailable);
    }
}