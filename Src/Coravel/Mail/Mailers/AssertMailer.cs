using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;
using System;
using Coravel.Mail;

namespace  Coravel.Mail.Mailers
{
    public class AssertMailer : IMailer
    {
        public class Data
        {
            public string message { get; set; }
            public string subject { get; set; }
            public IEnumerable<MailRecipient> to { get; set; }
            public MailRecipient from {get;set;}
            public MailRecipient replyTo { get; set; }
            public IEnumerable<MailRecipient> cc { get; set; }
            public IEnumerable<MailRecipient> bcc { get; set; }
        }

        private Action<Data> _assertAction;

        public AssertMailer(Action<Data> assertAction)
        {
            this._assertAction = assertAction;
        }

        public Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc)
        {
            this._assertAction(new Data
            {
                message = message,
                subject = subject,
                to = to,
                from = from,
                replyTo = replyTo,
                cc = cc,
                bcc = bcc
            });
            return Task.CompletedTask;
        }

        public async Task SendAsync<T>(Mailable<T> mailable) =>
            await mailable.SendAsync(null, this);
        

        public async Task<string> RenderAsync<T>(Mailable<T> mailable) => 
            await mailable.RenderAsync(null, this);
    }
}