using Coravel.Mailer.Mail;
using TestMvcApp.Models;

namespace TestMvcApp.Mailables
{
    public class NewUserViewMail : Mailable<UserModel>
    {
        private UserModel _user;

        public NewUserViewMail(UserModel user) => this._user = user;

        public override void Build()
        {
            this.To(this._user)
                .From("replyto@test.com")
                .View("~/Views/Mail/NewUser.cshtml", this._user);
        }
    }
}