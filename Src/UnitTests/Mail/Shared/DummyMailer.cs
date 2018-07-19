using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;
using System;

namespace UnitTests.Mail.Shared
{
    public class DummyMailer : IMailer
    {
        public class Data
        {
            public string message { get; set; }
            public string subject { get; set; }
            public IEnumerable<string> to { get; set; }
            public string replyTo { get; set; }
            public IEnumerable<string> cc { get; set; }
            public IEnumerable<string> bcc { get; set; }
        }

        private Action<Data> _assertAction;

        public DummyMailer(Action<Data> assertAction)
        {
            this._assertAction = assertAction;
        }

        public IRazorViewToStringRenderer GetViewRenderer()
        {
            return null; // Not needed for html mail.
        }

        public Task SendAsync(string message, string subject, IEnumerable<string> to, string replyTo, IEnumerable<string> cc, IEnumerable<string> bcc)
        {
            this._assertAction(new Data
            {
                message = message,
                subject = subject,
                to = to,
                replyTo = replyTo,
                cc = cc,
                bcc = bcc
            });
            return Task.CompletedTask;
        }
    }
}