using Coravel.Mailer.Mail;
using TestMvcApp.Models;

namespace TestMvcApp.Mailables;

public class WelcomeUserMailable : Mailable<string>
{
    private readonly UserModel _user;

    public WelcomeUserMailable(UserModel user) => _user = user;

    public override void Build()
    {
        To(_user)
            .From("replyto@test.com")
            .Html($"<html><body><h1>Welcome {_user.Name}</h1></body></html>");
    }
}