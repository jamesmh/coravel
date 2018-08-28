using Coravel.Mail;
using Demo.Models;

namespace Demo.Mailables
{
    public class NightlyReportMailable : Mailable<UserModel>
    {
        private UserModel _user;

        public NightlyReportMailable(UserModel user) => this._user = user;

        public override void Build()
        {
            this.To(this._user)
                .From("from@test.com")
                .View("~/Views/Mail/NewUser.cshtml", this._user);
        }
    }
}