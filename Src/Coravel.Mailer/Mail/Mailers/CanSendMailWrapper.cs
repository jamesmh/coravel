using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mailer.Mail.Interfaces;
using Coravel.Mailer.Mail.Renderers;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel.Mailer.Mail.Mailers
{
    public class CanSendMailWrapper<TCanSendMail> : IMailer where TCanSendMail : ICanSendMail
    {
        private RazorRenderer _renderer;
        private MailRecipient _globalFrom;
        private IServiceScopeFactory _scopeFactory;

        public CanSendMailWrapper(RazorRenderer renderer, IServiceScopeFactory scopeFactory, MailRecipient globalFrom = null)
        {
            this._renderer = renderer;
            this._scopeFactory = scopeFactory;
            this._globalFrom = globalFrom;
        }

        public Task<string> RenderAsync<T>(Mailable<T> mailable) =>
            mailable.RenderAsync(this._renderer, this);

        public async Task SendAsync<T>(Mailable<T> mailable) =>
            await mailable.SendAsync(this._renderer, this);

        public async Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments, MailRecipient sender = null)
        {
            await using (var scope = this._scopeFactory.CreateAsyncScope())
            {
                var canSendMail = scope.ServiceProvider.GetRequiredService<TCanSendMail>();
                
                await canSendMail.SendAsync(
                    message, subject, to, from ?? this._globalFrom, replyTo, cc, bcc, attachments, sender: sender
                );
            }
        }
    }
}