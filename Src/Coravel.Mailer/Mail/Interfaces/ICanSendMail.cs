using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mailer.Mail;

public interface ICanSendMail
{
    Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments = null, MailRecipient sender = null);
}