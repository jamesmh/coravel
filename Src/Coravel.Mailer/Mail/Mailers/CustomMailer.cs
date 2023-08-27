using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mailer.Mail.Interfaces;
using Coravel.Mailer.Mail.Renderers;

namespace Coravel.Mailer.Mail.Mailers;

public class CustomMailer : IMailer
{
    public delegate Task SendAsyncFunc(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient? replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment>? attachments = null);
    private readonly RazorRenderer? _renderer;
    private readonly SendAsyncFunc _sendAsyncFunc;
    private readonly MailRecipient? _globalFrom;

    public CustomMailer(RazorRenderer? renderer, SendAsyncFunc sendAsyncFunc, MailRecipient? globalFrom = null)
    {
        _renderer = renderer;
        _sendAsyncFunc = sendAsyncFunc;
        _globalFrom = globalFrom;
    }

    public Task<string> RenderAsync<T>(Mailable<T> mailable) =>
        mailable.RenderAsync(_renderer);

    public async Task SendAsync<T>(Mailable<T> mailable) =>
        await mailable.SendAsync(_renderer, this);

    public async Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient? from, MailRecipient? replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment>? attachments = null)
    {
        if (_globalFrom is null && from is null)
        {
            throw new System.ArgumentNullException(nameof(from), $"Both {nameof(from)} and {nameof(_globalFrom)} could not be null");
        }

        await _sendAsyncFunc(
            message, subject, to, _globalFrom ?? from!, replyTo, cc, bcc, attachments
        );
    }
}