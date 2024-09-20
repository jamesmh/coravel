using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Mailer.Mail.Helpers;
using Coravel.Mailer.Mail.Interfaces;
using Coravel.Mailer.Mail.Renderers;

namespace Coravel.Mailer.Mail.Mailers
{
    public class FileLogMailer : IMailer
    {
        private static readonly string FilePath = "mail.log";
        private RazorRenderer _renderer;
        private MailRecipient _globalFrom;

        public FileLogMailer(RazorRenderer renderer, MailRecipient globalFrom)
        {
            this._renderer = renderer;
            this._globalFrom = globalFrom;
        }

        public RazorRenderer GetViewRenderer() => this._renderer;

        public async Task<string> RenderAsync<T>(Mailable<T> mailable)
        {
            return await mailable.RenderAsync(this._renderer, this);
        }

        public async Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments = null, MailRecipient sender = null)
        {
            from = from ?? this._globalFrom;

            using (var writer = File.CreateText(FilePath))
            {
                await writer.WriteAsync($@"
---------------------------------------------
Subject: {subject}
To: {CommaSeparated(to)}    
From: {DisplayAddress(from)}
Sender: { (sender is null ? "N/A" : DisplayAddress(sender)) }
ReplyTo: {DisplayAddress(replyTo)}
Cc: {CommaSeparated(cc)}
Bcc: {CommaSeparated(bcc)}
Attachment: { (attachments is null ? "N/A" : string.Join(";", attachments.Select(a => a.Name))) }
---------------------------------------------

{message}
                ").ConfigureAwait(false);
            }
        }

        public async Task SendAsync<T>(Mailable<T> mailable)
        {
            await mailable.SendAsync(this._renderer, this);
        }

        private static string CommaSeparated(IEnumerable<MailRecipient> recipients) =>
            (recipients ?? Enumerable.Empty<MailRecipient>())
                .Select(r => DisplayAddress(r))
                .CommaSeparated();

        private static string DisplayAddress(MailRecipient recipient)
        {
            if (recipient == null)
                return string.Empty;
            else
                return $"{recipient.Name} <{recipient.Email}>";
        }
    }
}