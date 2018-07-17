using Coravel.Mail;
using TestMvcApp.Models;

namespace TestMvcApp.Mailables
{
    public class NewUserFromViewMailable : Mailable<UserModel>
    {
        private UserModel _user;

        public NewUserFromViewMailable(UserModel user) => this._user = user;

        public override void Build()
        {
            this.To(this._user.Email)
                .From("replyto@test.com")
                .View("~/Views/Mail/NewUser.cshtml", this._user);
        }
    }
}