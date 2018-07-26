using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;

namespace Coravel.Mail.Mailers
{
    public class CustomMailer : IMailer
    {
        public delegate Task SendAsyncFunc(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc);
        private IRazorRenderer _renderer;
        private SendAsyncFunc _sendAsyncFunc;

        public CustomMailer(IRazorRenderer renderer, SendAsyncFunc sendAsyncFunc){
             this._renderer = renderer;
             this._sendAsyncFunc = sendAsyncFunc;
        }

        public Task<string> Render<T>(Mailable<T> mailable) =>
            mailable.Render(this._renderer, this);        

        public async Task SendAsync<T>(Mailable<T> mailable) =>
            await mailable.SendAsync(this._renderer, this);

        public async Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc)
        {
            await this._sendAsyncFunc(
                message, subject, to, from, replyTo, cc, bcc               
            );
        }
    }
}