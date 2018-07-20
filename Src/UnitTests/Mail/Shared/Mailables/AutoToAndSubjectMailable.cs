using Coravel.Mail;
using UnitTests.Mail.Shared.Models;

namespace UnitTests.Mail.Shared.Mailables
{
    public class AutoToAndSubjectMailable : Mailable<TestUser>
    {
        private TestUser _user;

        public AutoToAndSubjectMailable(TestUser user) {
            this._user = user;
        }

        public override void Build()
        {
            this.To(this._user)
                .From("from@test.com")
                .Html($"<html><body>Hi {this._user.Name}</body></html>");
        }
    }
}