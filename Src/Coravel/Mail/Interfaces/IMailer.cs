using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coravel.Mail.Interfaces
{
    public interface IMailer
    {
        Task SendAsync(string message, string subject, IEnumerable<string> to, string replyTo, IEnumerable<string> cc, IEnumerable<string> bcc);      
        IRazorViewToStringRenderer GetViewRenderer();
    }
}