using Coravel.Mailer.Mail;
using TestMvcApp.Models;

namespace TestMvcApp.Mailables
{
    public class WelcomeUserMailable : Mailable<string>
    {
        private UserModel _user;

        public WelcomeUserMailable(UserModel user) => this._user = user;

        public override void Build()
        {
            this.To(this._user)
                .From("replyto@test.com")
                .Html($"<html><body><h1>Welcome {this._user.Name}</h1></body></html>");
        }
    }
}