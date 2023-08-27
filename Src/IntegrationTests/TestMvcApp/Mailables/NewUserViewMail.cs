using Coravel.Mailer.Mail;
using TestMvcApp.Models;

namespace TestMvcApp.Mailables;

public class NewUserViewMail : Mailable<UserModel>
{
    private readonly UserModel _user;

    public NewUserViewMail(UserModel user) => _user = user;

    public override void Build()
    {
        To(_user)
            .From("replyto@test.com")
            .View("~/Views/Mail/NewUser.cshtml", _user);
    }
}