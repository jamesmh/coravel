using Coravel.Mail;
using UnitTests.Mail.Shared.Models;

namespace UnitTests.Mail.Shared.Mailables
{
    public class MailableWithModelProperties : Mailable<TestUserWithFields>
    {
        private TestUser _user;

        public MailableWithModelProperties(TestUser user) {
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