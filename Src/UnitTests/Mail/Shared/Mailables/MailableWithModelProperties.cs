using Coravel.Mailer.Mail;
using UnitTests.Mail.Shared.Models;

namespace UnitTests.Mail.Shared.Mailables
{
    public class MailableWithModelFields : Mailable<TestUserWithFields>
    {
        private TestUserWithFields _user;

        public MailableWithModelFields(TestUserWithFields user)
        {
            this._user = user;
        }

        public override void Build()
        {
            this.To(this._user)
                .From("from@test.com")
                .Html($"<html><body>Hi</body></html>");
        }
    }
}