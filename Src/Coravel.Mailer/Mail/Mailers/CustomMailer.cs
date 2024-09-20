using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mailer.Mail.Interfaces;
using Coravel.Mailer.Mail.Renderers;

namespace Coravel.Mailer.Mail.Mailers
{
    public class CustomMailer : IMailer
    {
        public delegate Task SendAsyncFunc(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments = null, MailRecipient sender = null);
        private RazorRenderer _renderer;
        private SendAsyncFunc _sendAsyncFunc;
        private MailRecipient _globalFrom;

        public CustomMailer(RazorRenderer renderer, SendAsyncFunc sendAsyncFunc, MailRecipient globalFrom = null)
        {
            this._renderer = renderer;
            this._sendAsyncFunc = sendAsyncFunc;
            this._globalFrom = globalFrom;
        }

        public Task<string> RenderAsync<T>(Mailable<T> mailable) =>
            mailable.RenderAsync(this._renderer, this);

        public async Task SendAsync<T>(Mailable<T> mailable) =>
            await mailable.SendAsync(this._renderer, this);

        public async Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments, MailRecipient sender = null)
        {
            await this._sendAsyncFunc(
                message, subject, to, from ?? this._globalFrom, replyTo, cc, bcc, attachments, sender: sender
            );
        }
    }
}