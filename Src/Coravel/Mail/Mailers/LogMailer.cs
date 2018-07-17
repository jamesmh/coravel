using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;

namespace Coravel.Mail.Mailers
{
    public class LogMailer : IMailer
    {
        private IRazorViewToStringRenderer _razorToStringRenderer;
        
        public LogMailer(IRazorViewToStringRenderer razorToStringRenderer)
        {
            this._razorToStringRenderer = razorToStringRenderer;
        }

        public IRazorViewToStringRenderer GetViewRenderer() => this._razorToStringRenderer;

        public Task SendAsync(string message, string subject, IEnumerable<string> to, string replyTo, IEnumerable<string> cc, IEnumerable<string> bcc)
        {
            Debug.Write(message);
            return Task.CompletedTask;
        }
    }
}