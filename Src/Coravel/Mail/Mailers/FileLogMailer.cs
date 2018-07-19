using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;
using Coravel.Mail.Renderers;

namespace Coravel.Mail.Mailers
{
    public class FileLogMailer : IMailer
    {
        private static readonly string FilePath = "mail.log";
        private IRazorViewToStringRenderer _razorToStringRenderer;

        public FileLogMailer(IRazorViewToStringRenderer razorToStringRenderer)
        {
            this._razorToStringRenderer = razorToStringRenderer;
        }

        public IRazorViewToStringRenderer GetViewRenderer() => this._razorToStringRenderer;

        public async Task SendAsync(string message, string subject, IEnumerable<string> to, string from, string replyTo, IEnumerable<string> cc, IEnumerable<string> bcc)
        {
            using (var writer = File.CreateText(FilePath))
            {
                await writer.WriteAsync($@"
                    ---------------------------------------------
                    Subject: {subject}
                    To: {CommaSeparated(to)}    
                    From: {from}
                    ReplyTo: {replyTo}
                    Cc: {CommaSeparated(cc)}
                    Bcc: {CommaSeparated(bcc)}
                    ---------------------------------------------

                    {message}
                ").ConfigureAwait(false);
            }
        }

        private static string CommaSeparated(IEnumerable<string> str)
        {
            if (str == null)
            {
                return string.Empty;
            }

            return string.Join(",", str);
        }

        public async Task SendAsync<T>(Mailable<T> mailable)
        {
            await mailable.SendAsync(this);
        }
    }
}