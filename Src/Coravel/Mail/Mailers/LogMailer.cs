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
            await this.WriteToFile(message);
        }

        private async Task WriteToFile(string message)
        {
            using(var writer = File.CreateText(FilePath))
            {
                await writer.WriteAsync(message);
            }
        }
    }
}