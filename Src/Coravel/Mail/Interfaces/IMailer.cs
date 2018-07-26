using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mail.Renderers;

namespace Coravel.Mail.Interfaces
{
    public interface IMailer
    {
        Task<string> Render<T>(Mailable<T> mailable);
        Task SendAsync<T>(Mailable<T> mailable);
        Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc);      
    }
}