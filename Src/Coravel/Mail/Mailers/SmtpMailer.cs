using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System;
using System.Net.Security;
using System.Linq;
using Coravel.Mail.Renderers;

namespace Coravel.Mail.Mailers
{
    public class SmtpMailer : IMailer
    {
        private RazorRenderer _renderer;
        private string _host;
        private int _port;
        private string _username;
        private string _password;
        private MailRecipient _globalFrom;
        private RemoteCertificateValidationCallback _certCallback;

        public SmtpMailer(
            RazorRenderer renderer,
            string host,
            int port,
            string username,
            string password,
            MailRecipient globalFrom = null,
            RemoteCertificateValidationCallback certificateCallback = null)
        {
            this._renderer = renderer;
            this._host = host;
            this._port = port;
            this._username = username;
            this._password = password;
            this._globalFrom = globalFrom;

            this._certCallback = certificateCallback;
            if (this._certCallback == null)
            {
                this._certCallback = (s, c, h, e) => true; // Allow any cert.
            }
        }

        public Task<string> RenderAsync<T>(Mailable<T> mailable) => 
            mailable.RenderAsync(this._renderer, this);

        public async Task SendAsync<T>(Mailable<T> mailable)
        {
            await mailable.SendAsync(this._renderer, this);
        }

        public async Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc)
        {
            var mail = new MimeMessage();
            mail.From.Add(AsMailboxAddress(this._globalFrom ?? from));

            foreach (var recipientAddress in to ?? Enumerable.Empty<MailRecipient>())
            {
                mail.To.Add(AsMailboxAddress(recipientAddress));
            }

            foreach (var ccReciepient in cc ?? Enumerable.Empty<MailRecipient>())
            {
                mail.Cc.Add(AsMailboxAddress(ccReciepient));
            }

            foreach (var bccReciepient in bcc ?? Enumerable.Empty<MailRecipient>())
            {
                mail.Bcc.Add(AsMailboxAddress(bccReciepient));
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

        private static MailboxAddress AsMailboxAddress(MailRecipient recipient) =>
            new MailboxAddress(recipient.Name, recipient.Email);
    }
}