using Coravel.Mail;
using UnitTests.Mail.Shared.Models;

namespace UnitTests.Mail.Shared.Mailables
{
    public class GenericHtmlMailable : Mailable<string>
    {

        public GenericHtmlMailable()
        {
        }

        public override void Build()
        {
            // For testing, methods issued by caller.
        }
    }
}