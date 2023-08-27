using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Coravel.Mailer.Mail.Interfaces;
using System.Linq;

namespace Coravel.Mailer.Mail.Mailers;

public class AssertMailer : IMailer
{
    public class Data
    {
        public string Message { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public IEnumerable<MailRecipient> To { get; set; } = Enumerable.Empty<MailRecipient>();
        public MailRecipient? From { get; set; }
        public MailRecipient? ReplyTo { get; set; }
        public IEnumerable<MailRecipient> Cc { get; set; } = Enumerable.Empty<MailRecipient>();
        public IEnumerable<MailRecipient> Bcc { get; set; } = Enumerable.Empty<MailRecipient>();
        public IEnumerable<Attachment>? Attachments { get; set; }
    }

    private readonly Action<Data> _assertAction;

    public AssertMailer(Action<Data> assertAction) => _assertAction = assertAction;

    public Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient? from, MailRecipient? replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment>? attachments = null)
    {
        _assertAction(new Data
        {
            Message = message,
            Subject = subject,
            To = to,
            From = from,
            ReplyTo = replyTo,
            Cc = cc,
            Bcc = bcc,
            Attachments = attachments
        });
        return Task.CompletedTask;
    }

    public async Task SendAsync<T>(Mailable<T> mailable) =>
        await mailable.SendAsync(null, this);


    public async Task<string> RenderAsync<T>(Mailable<T> mailable) =>
        await mailable.RenderAsync();
}