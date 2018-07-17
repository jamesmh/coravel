using Coravel.Mail;
using TestMvcApp.Models;

namespace TestMvcApp.Mailables
{
    public class NewUserFromHtmlMailable : Mailable<string>
    {
        private UserModel _user;

        public NewUserFromHtmlMailable(UserModel user) => this._user = user;

        public override void Build()
        {
            this.To(this._user.Email)
                .From("replyto@test.com")
                .Html($"<html><body><h1>Welcome {this._user.Name}</h1></body></html>");
        }
    }
}