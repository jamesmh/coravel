using Coravel.Mailer.Mail;
using Demo.Models;

namespace Demo.Mailables
{
    public class WelcomeUserHtmlMail : Mailable<string>
    {
        private UserModel _user;

        public WelcomeUserHtmlMail(UserModel user) => this._user = user;

        public override void Build()
        {
            this.To(this._user.Email)
                .From("replyto@test.com")
                .Html($"<html><body><h1>Welcome {this._user.Name}</h1></body></html>");
        }
    }
}