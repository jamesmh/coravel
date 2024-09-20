using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Security;
using System.Linq;
using Coravel.Mailer.Mail.Interfaces;
using Coravel.Mailer.Mail.Renderers;

namespace Coravel.Mailer.Mail.Mailers
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

        public async Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments = null, MailRecipient sender = null)
        {
            var mail = new MimeMessage();
            
            this.SetFrom(@from, mail);
            SetSender(sender, mail);
            SetRecipients(to, mail);
            SetCc(cc, mail);
            SetBcc(bcc, mail);
            mail.Subject = subject;
            SetMailBody(message, attachments, mail);

            if (replyTo != null)
            {
                SetReplyTo(replyTo, mail);
            }

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = this._certCallback;
                await client.ConnectAsync(this._host, this._port).ConfigureAwait(false);

                if (this.UseSMTPAuthentication())
                {
                    await client.AuthenticateAsync(this._username, this._password);
                }

                await client.SendAsync(mail).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }

        private static void SetMailBody(string message, IEnumerable<Attachment> attachments, MimeMessage mail)
        {
            var bodyBuilder = new BodyBuilder { HtmlBody = message };
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    bodyBuilder.Attachments.Add(attachment.Name, attachment.Bytes);
                }
            }

            mail.Body = bodyBuilder.ToMessageBody();
        }

        private static void SetBcc(IEnumerable<MailRecipient> bcc, MimeMessage mail)
        {
            foreach (var bccRecipient in bcc ?? Enumerable.Empty<MailRecipient>())
            {
                mail.Bcc.Add(AsMailboxAddress(bccRecipient));
            }
        }

        private static void SetCc(IEnumerable<MailRecipient> cc, MimeMessage mail)
        {
            foreach (var ccRecipient in cc ?? Enumerable.Empty<MailRecipient>())
            {
                mail.Cc.Add(AsMailboxAddress(ccRecipient));
            }
        }

        private static void SetRecipients(IEnumerable<MailRecipient> to, MimeMessage mail)
        {
            foreach (var recipientAddress in to ?? Enumerable.Empty<MailRecipient>())
            {
                mail.To.Add(AsMailboxAddress(recipientAddress));
            }
        }

        private void SetFrom(MailRecipient @from, MimeMessage mail)
        {
            mail.From.Add(AsMailboxAddress(@from ?? this._globalFrom));
        }

        private static void SetSender(MailRecipient sender, MimeMessage mail)
        {
            if(sender is null)
            {
                return;
            }
            
            mail.Sender = AsMailboxAddress(sender);            
        }

        private static void SetReplyTo(MailRecipient replyTo, MimeMessage mail)
        {
            mail.ReplyTo.Add(AsMailboxAddress(replyTo));
        }

        private static MailboxAddress AsMailboxAddress(MailRecipient recipient) =>
            new MailboxAddress(recipient.Name, recipient.Email);

        public bool UseSMTPAuthentication() {
            bool bypassAuth = string.IsNullOrEmpty(this._username)
                && string.IsNullOrEmpty(this._password);
            return !bypassAuth;
        }
    }
}