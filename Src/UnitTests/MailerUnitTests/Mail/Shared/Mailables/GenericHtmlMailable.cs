using Coravel.Mailer.Mail;

namespace UnitTests.Mail.Shared.Mailables;

public class GenericHtmlMailable : Mailable<string>
{
    public override void Build()
    {
        // For testing, methods issued by caller.
    }
}