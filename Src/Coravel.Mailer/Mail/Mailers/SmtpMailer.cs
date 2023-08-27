using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net.Security;
using System.Linq;
using Coravel.Mailer.Mail.Interfaces;
using Coravel.Mailer.Mail.Renderers;

namespace Coravel.Mailer.Mail.Mailers;

public class SmtpMailer : IMailer
{
    private readonly RazorRenderer _renderer;
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly MailRecipient? _globalFrom;
    private readonly RemoteCertificateValidationCallback? _certCallback;

    public SmtpMailer(
        RazorRenderer renderer,
        string host,
        int port,
        string username,
        string password,
        MailRecipient? globalFrom = null,
        RemoteCertificateValidationCallback? certificateCallback = null)
    {
        _renderer = renderer;
        _host = host;
        _port = port;
        _username = username;
        _password = password;
        _globalFrom = globalFrom;

        _certCallback = certificateCallback;

        if (_certCallback == null)
        {
            _certCallback = (s, c, h, e) => true; // Allow any cert.
        }
    }

    public Task<string> RenderAsync<T>(Mailable<T> mailable) =>
        mailable.RenderAsync(_renderer);

    public async Task SendAsync<T>(Mailable<T> mailable)
    {
        await mailable.SendAsync(_renderer, this);
    }

    public async Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient? from, MailRecipient? replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment>? attachments = null)
    {
        var mail = new MimeMessage();

        SetFrom(@from, mail);
        SetRecipients(to, mail);
        SetCc(cc, mail);
        SetBcc(bcc, mail);
        mail.Subject = subject;
        SetMailBody(message, attachments, mail);

        using var client = new SmtpClient();
        client.ServerCertificateValidationCallback = _certCallback;
        await client.ConnectAsync(_host, _port).ConfigureAwait(false);

        if (UseSMTPAuthentication())
        {
            await client.AuthenticateAsync(_username, _password);
        }

        await client.SendAsync(mail).ConfigureAwait(false);
        await client.DisconnectAsync(true).ConfigureAwait(false);
    }

    private static void SetMailBody(string message, IEnumerable<Attachment>? attachments, MimeMessage mail)
    {
        var bodyBuilder = new BodyBuilder { HtmlBody = message };

        if (attachments != null)
        {
            foreach (var attachment in attachments)
            {
                bodyBuilder.Attachments.Add(attachment.Name, attachment.Bytes);
            }
        }

        mail.Body = bodyBuilder.ToMessageBody();
    }

    private static void SetBcc(IEnumerable<MailRecipient> bcc, MimeMessage mail)
    {
        foreach (var bccRecipient in bcc ?? Enumerable.Empty<MailRecipient>())
        {
            mail.Bcc.Add(AsMailboxAddress(bccRecipient));
        }
    }

    private static void SetCc(IEnumerable<MailRecipient> cc, MimeMessage mail)
    {
        foreach (var ccRecipient in cc ?? Enumerable.Empty<MailRecipient>())
        {
            mail.Cc.Add(AsMailboxAddress(ccRecipient));
        }
    }

    private static void SetRecipients(IEnumerable<MailRecipient> to, MimeMessage mail)
    {
        foreach (var recipientAddress in to ?? Enumerable.Empty<MailRecipient>())
        {
            mail.To.Add(AsMailboxAddress(recipientAddress));
        }
    }

    private void SetFrom(MailRecipient? @from, MimeMessage mail)
    {
        mail.From.Add(AsMailboxAddress(@from ?? _globalFrom ?? throw new System.ArgumentNullException(nameof(@from))));
    }

    private static MailboxAddress AsMailboxAddress(MailRecipient recipient) =>
        new(recipient.Name, recipient.Email);

    public bool UseSMTPAuthentication()
    {
        bool bypassAuth = string.IsNullOrEmpty(_username)
            && string.IsNullOrEmpty(_password);
        return !bypassAuth;
    }
}