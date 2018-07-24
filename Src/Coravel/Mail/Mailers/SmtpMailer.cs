using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System;
using System.Net.Security;
using System.Linq;

namespace Coravel.Mail.Mailers
{
    public class SmtpMailer : IMailer
    {
        private IRazorRenderer _renderer;
        private string _host;
        private int _port;
        private string _username;
        private string _password;
        private RemoteCertificateValidationCallback _certCallback;

        public SmtpMailer(
            IRazorRenderer renderer,
            string host,
            int port,
            string username,
            string password,
            RemoteCertificateValidationCallback certificateCallback)
        {
            this._renderer = renderer;
            this._host = host;
            this._port = port;
            this._username = username;
            this._password = password;

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

            foreach (var ccReciepient in cc ?? Enumerable.Empty<string>())
            {
                mail.Cc.Add(new MailboxAddress(ccReciepient));
            }

            foreach (var bccReciepient in bcc ?? Enumerable.Empty<string>())
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

                if (this._username != null && this._password != null)
                {
                    await client.AuthenticateAsync(this._username, this._password);
                }

                await client.SendAsync(mail).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}