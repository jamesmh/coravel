using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coravel.Mailer.Mail.Interfaces
{
    public interface IMailer
    {
        Task<string> RenderAsync<T>(Mailable<T> mailable);
        Task SendAsync<T>(Mailable<T> mailable);
        Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, MailRecipient sender, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments = null);
    }
}