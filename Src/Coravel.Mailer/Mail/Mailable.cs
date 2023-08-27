using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Mailer.Mail.Exceptions;
using Coravel.Mailer.Mail.Helpers;
using Coravel.Mailer.Mail.Interfaces;
using Coravel.Mailer.Mail.Renderers;

namespace Coravel.Mailer.Mail;

public class Mailable<T>
{
    private const string NoRenderFoundMessage = "Please use one of the available methods for specifying how to render your mail (e.g. Html() or View())";

    /// <summary>
    /// Who the email is from.
    /// </summary>
    private MailRecipient? _from;

    /// <summary>
    /// Recipients of the message.
    /// </summary>
    private IEnumerable<MailRecipient> _to = Enumerable.Empty<MailRecipient>();

    /// <summary>
    /// cc recipients of the message.
    /// </summary>
    private IEnumerable<MailRecipient> _cc = Enumerable.Empty<MailRecipient>();

    /// <summary>
    /// bcc recipients of the message.
    /// </summary>
    private IEnumerable<MailRecipient> _bcc = Enumerable.Empty<MailRecipient>();

    /// <summary>
    /// Who the recipients should reply to.
    /// </summary>
    private MailRecipient? _replyTo;

    /// <summary>
    /// Mail's subject.
    /// </summary>
    private string _subject = string.Empty;

    /// <summary>
    /// Raw HTML to use as email message.
    /// </summary>
    private string _html = string.Empty;

    /// <summary>
    /// The MVC view to use to generate the message.
    /// </summary>
    private string _viewPath = string.Empty;

    /// <summary>
    /// Model that we want to mail to.
    /// </summary>
    private object? _mailToModel;

    private List<Attachment>? _attachments;

    /// <summary>
    /// View data to pass to the view to render.
    /// </summary>
    private T? _viewModel;

    public Mailable<T> From(MailRecipient recipient)
    {
        _from = recipient;
        return this;
    }

    public Mailable<T> From(string email) =>
        From(new MailRecipient(email));

    public Mailable<T> To(IEnumerable<MailRecipient> recipients)
    {
        _to = recipients;
        return this;
    }

    public Mailable<T> To(MailRecipient recipient) =>
        To(new MailRecipient[] { recipient });

    public Mailable<T> To(IEnumerable<string> addresses) =>
        To(addresses.Select(address => new MailRecipient(address)));

    public Mailable<T> To(string email) =>
        To(new MailRecipient(email));

    public Mailable<T> To(object mailToModel)
    {
        _mailToModel = mailToModel;
        return this;
    }

    public Mailable<T> Cc(IEnumerable<MailRecipient> recipients)
    {
        _cc = recipients;
        return this;
    }

    public Mailable<T> Cc(IEnumerable<string> addresses) =>
        Cc(addresses.Select(address => new MailRecipient(address)));

    public Mailable<T> Bcc(IEnumerable<MailRecipient> recipients)
    {
        _bcc = recipients;
        return this;
    }

    public Mailable<T> Bcc(IEnumerable<string> addresses) =>
        Bcc(addresses.Select(address => new MailRecipient(address)));

    public Mailable<T> ReplyTo(MailRecipient replyTo)
    {
        _replyTo = replyTo;
        return this;
    }

    public Mailable<T> ReplyTo(string address)
    {
        _replyTo = new MailRecipient(address);
        return this;
    }

    public Mailable<T> Subject(string subject)
    {
        _subject = subject;
        return this;
    }

    public Mailable<T> Attach(Attachment attachment)
    {
        if (_attachments is null)
        {
            _attachments = new List<Attachment>();
        }
        _attachments.Add(attachment);
        return this;
    }

    public Mailable<T> Html(string html)
    {
        _html = html;
        return this;
    }

    public Mailable<T> View(string viewPath, T? viewModel)
    {
        _viewModel = viewModel;
        _viewPath = viewPath;
        return this;
    }

    public Mailable<T> View(string viewPath)
    {
        View(viewPath, default);
        return this;
    }

    public virtual void Build() { }

    internal async Task SendAsync(RazorRenderer? renderer, IMailer mailer)
    {
        Build();

        var message = await BuildMessage(renderer).ConfigureAwait(false);

        await mailer.SendAsync(
            message,
            _subject,
            _to,
            _from,
            _replyTo,
            _cc,
            _bcc,
            _attachments
        ).ConfigureAwait(false);
    }

    internal async Task<string> RenderAsync(RazorRenderer? renderer = null)
    {
        if (renderer != null)
        {
            Build();
            return await BuildMessage(renderer).ConfigureAwait(false);
        }

        return string.Empty;
    }

    private async Task<string> BuildMessage(RazorRenderer? renderer)
    {
        BindDynamicProperties();

        if (_html != null)
        {
            return _html;
        }

        if (_viewPath != null && renderer != null)
        {
            return await renderer
                .RenderViewToStringAsync(_viewPath, _viewModel)
                .ConfigureAwait(false);
        }

        throw new NoMailRendererFound(NoRenderFoundMessage);
    }

    private void BindDynamicProperties()
    {
        if (HasMailToModel())
        {
            BindEmailField();
        }

        if (HasNoSubject())
        {
            BindSubjectField();
        }
    }

    private bool HasNoSubject() => _subject == null;

    private bool HasMailToModel() => _mailToModel != null;

    private void BindEmailField()
    {
        object? propEmail = _mailToModel?.GetPropOrFieldValue("Email");
        object? propName = _mailToModel?.GetPropOrFieldValue("Name");

        if (propEmail is string address)
        {
            if (propName is string name)
            {
                To(new MailRecipient(address, name));
            }
            else
            {
                To(address);
            }
        }
    }

    private void BindSubjectField()
    {
        if (_subject == null)
        {
            var classNameOfThisMailable = GetType().Name;

            _subject = classNameOfThisMailable
                .ToSnakeCase()
                .RemoveLastOccuranceOfWord("Mailable");
        }
    }
}