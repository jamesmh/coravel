using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System;
using System.Net.Security;

namespace Coravel.Mail.Mailers
{
    public class SmtpMailer : IMailer
    {
        private IRazorRenderer _renderer;
        private string _host;
        private int _port;
        private RemoteCertificateValidationCallback _certCallback;

        public SmtpMailer(IRazorRenderer razorToStringRenderer, string host, int port, RemoteCertificateValidationCallback certificateCallback = null)
        {
            this._renderer = razorToStringRenderer;
            this._host = host;
            this._port = port;

            this._certCallback = certificateCallback;
            if (this._certCallback == null)
            {
                this._certCallback = (s, c, h, e) => true; // Allow any cert.
            }
        }

        public Task<string> Render<T>(Mailable<T> mailable) => mailable.Render(this._renderer, this);

        public async Task SendAsync<T>(Mailable<T> mailable)
        {
            await mailable.SendAsync(this._renderer, this);
        }

        public async Task SendAsync(string message, string subject, IEnumerable<string> to, string from, string replyTo, IEnumerable<string> cc, IEnumerable<string> bcc)
        {
            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress(from));

            foreach (var recipientAddress in to)
            {
                mail.To.Add(new MailboxAddress(recipientAddress));
            }

            foreach (var ccReciepient in cc)
            {
                mail.Cc.Add(new MailboxAddress(ccReciepient));
            }

            foreach (var bccReciepient in cc)
            {
                mail.Bcc.Add(new MailboxAddress(bccReciepient));
            }

            mail.Subject = subject;

            mail.Body = new TextPart(TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {

                client.ServerCertificateValidationCallback = this._certCallback;

                await client.ConnectAsync(this._host, this._port).ConfigureAwait(false);
                await client.SendAsync(mail).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}