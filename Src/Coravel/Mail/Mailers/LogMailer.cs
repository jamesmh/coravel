using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;

namespace Coravel.Mail.Mailers
{
    public class LogMailer : IMailer
    {
        private static readonly string FilePath = "mail.log";
        private IRazorViewToStringRenderer _razorToStringRenderer;

        public LogMailer(IRazorViewToStringRenderer razorToStringRenderer)
        {
            this._razorToStringRenderer = razorToStringRenderer;
        }

        public IRazorViewToStringRenderer GetViewRenderer() => this._razorToStringRenderer;

        public async Task SendAsync(string message, string subject, IEnumerable<string> to, string replyTo, IEnumerable<string> cc, IEnumerable<string> bcc)
        {
            using (var writer = File.CreateText(FilePath))
            {
                await writer.WriteLineAsync($"Subject: {subject}");
                await writer.WriteLineAsync($"To: {CommaSeparated(to)}");
                await writer.WriteLineAsync($"ReplyTo: {replyTo}");
                await writer.WriteLineAsync($"Cc: {CommaSeparated(cc)}");
                await writer.WriteLineAsync($"Bcc: {CommaSeparated(bcc)}");
                await writer.WriteAsync(message);
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

    }
}