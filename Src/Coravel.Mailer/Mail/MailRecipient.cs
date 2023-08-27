namespace Coravel.Mailer.Mail;

public class MailRecipient
{
    public string Email { get; set; }
    public string? Name { get; set; }

    public MailRecipient(string email) => Email = email;

    public MailRecipient(string email, string name) : this(email) => Name = name;
}