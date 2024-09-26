namespace Coravel.Mailer.Mail;

public class MessageBody
{
    public string Html { get; set; }
    public string Text { get; set; }

    public MessageBody(string html, string text)
    {
        if(html is null && text is null)
        {
            throw new System.ArgumentException("You need to supply either an HTML or plain text message - or both.");
        }

        this.Html = html;
        this.Text = text;
    }

    public bool HasHtmlMessage() => this.Html != null;

    public bool HasPlainTextMessage() => this.Text != null;
}
