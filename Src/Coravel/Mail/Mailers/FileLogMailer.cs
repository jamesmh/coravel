using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;
using Coravel.Mail.Renderers;
using Coravel.Mail.Helpers;

namespace Coravel.Mail.Mailers
{
    public class FileLogMailer : IMailer
    {
        private static readonly string FilePath = "mail.log";
        private RazorRenderer _renderer;

        public FileLogMailer(RazorRenderer renderer)
        {
            this._renderer = renderer;
        }

        public RazorRenderer GetViewRenderer() => this._renderer;

        public async Task<string> Render<T>(Mailable<T> mailable)
        {
            return await mailable.Render(this._renderer, this);
        }

        public async Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc)
        {
            using (var writer = File.CreateText(FilePath))
            {
                await writer.WriteAsync($@"
                    ---------------------------------------------
                    Subject: {subject}
                    To: {CommaSeparated(to)}    
                    From: {from?.Name}<{from?.Email}>
                    ReplyTo: {replyTo?.Name}<{replyTo?.Email}>
                    Cc: {CommaSeparated(cc)}
                    Bcc: {CommaSeparated(bcc)}
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
                .Select(r => $"{r?.Name}<{r?.Email}>")
                .CommaSeparated();
    }
}