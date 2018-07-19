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
            this.To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Html($"<html><body>Hi!</body></html>");
        }
    }
}