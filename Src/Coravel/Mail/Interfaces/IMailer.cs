using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mail.Renderers;

namespace Coravel.Mail.Interfaces
{
    public interface IMailer
    {
        Task SendAsync<T>(Mailable<T> mailable);
        Task SendAsync(string message, string subject, IEnumerable<string> to, string from, string replyTo, IEnumerable<string> cc, IEnumerable<string> bcc);      
        IRazorViewToStringRenderer GetViewRenderer();
    }
}