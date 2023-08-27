using Coravel.Mailer.Mail;
using UnitTests.Mail.Shared.Models;

namespace UnitTests.Mail.Shared.Mailables;

public class MailableWithModelFields : Mailable<TestUserWithFields>
{
    private readonly TestUserWithFields _user;

    public MailableWithModelFields(TestUserWithFields user) => _user = user;

    public override void Build()
    {
        To(_user)
            .From("from@test.com")
            .Html($"<html><body>Hi</body></html>");
    }
}