using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;
using Coravel.Mail.Renderers;
using Coravel.Mail.Helpers;

namespace Coravel.Mail.Mailers
{
    public class FileLogMailer : IMailer
    {
        private static readonly string FilePath = "mail.log";
        private IRazorRenderer _renderer;

        public FileLogMailer(IRazorRenderer renderer)
        {
            this._renderer = renderer;
        }

        public IRazorRenderer GetViewRenderer() => this._renderer;

        public async Task<string> Render<T>(Mailable<T> mailable)
        {
            return await mailable.Render(this._renderer, this);
        }

        public async Task SendAsync(string message, string subject, IEnumerable<string> to, string from, string replyTo, IEnumerable<string> cc, IEnumerable<string> bcc)
        {
            using (var writer = File.CreateText(FilePath))
            {
                await writer.WriteAsync($@"
                    ---------------------------------------------
                    Subject: {subject}
                    To: {to.CommaSeparated()}    
                    From: {from}
                    ReplyTo: {replyTo}
                    Cc: {cc.CommaSeparated()}
                    Bcc: {bcc.CommaSeparated()}
                    ---------------------------------------------

                    {message}
                ").ConfigureAwait(false);
            }
        }

        public async Task SendAsync<T>(Mailable<T> mailable)
        {
            await mailable.SendAsync(this._renderer, this);
        }
    }
}