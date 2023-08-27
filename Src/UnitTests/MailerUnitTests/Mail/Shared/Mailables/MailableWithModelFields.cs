using Coravel.Mailer.Mail;
using UnitTests.Mail.Shared.Models;

namespace UnitTests.Mail.Shared.Mailables;

public class MailableWithModelProperties : Mailable<TestUserWithFields>
{
    private readonly TestUser _user;

    public MailableWithModelProperties(TestUser user) => _user = user;

    public override void Build()
    {
        To(_user)
            .From("from@test.com")
            .Html($"<html><body>Hi</body></html>");
    }
}