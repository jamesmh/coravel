using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Mailer.Mail.Helpers;
using Coravel.Mailer.Mail.Interfaces;
using Coravel.Mailer.Mail.Renderers;
using Microsoft.Extensions.Configuration;

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

        public async Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc)
        {

            from = this._globalFrom ?? from;

            using (var writer = File.CreateText(FilePath))
            {
                await writer.WriteAsync($@"
---------------------------------------------
Subject: {subject}
To: {CommaSeparated(to)}    
From: {DisplayAddress(from)}
ReplyTo: {DisplayAddress(replyTo)}
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