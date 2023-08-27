using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Mailer.Mail.Helpers;
using Coravel.Mailer.Mail.Interfaces;
using Coravel.Mailer.Mail.Renderers;

namespace Coravel.Mailer.Mail.Mailers;

public class FileLogMailer : IMailer
{
    private static readonly string FilePath = "mail.log";
    private readonly RazorRenderer _renderer;
    private readonly MailRecipient? _globalFrom;

    public FileLogMailer(RazorRenderer renderer, MailRecipient? globalFrom)
    {
        _renderer = renderer;
        _globalFrom = globalFrom;
    }

    public RazorRenderer GetViewRenderer() => _renderer;

    public async Task<string> RenderAsync<T>(Mailable<T> mailable)
    {
        return await mailable.RenderAsync(_renderer);
    }

    public async Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient? from, MailRecipient? replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment>? attachments = null)
    {
        from = _globalFrom ?? from;

        using var writer = File.CreateText(FilePath);
        await writer.WriteAsync($@"
---------------------------------------------
Subject: {subject}
To: {CommaSeparated(to)}    
From: {DisplayAddress(from)}
ReplyTo: {DisplayAddress(replyTo)}
Cc: {CommaSeparated(cc)}
Bcc: {CommaSeparated(bcc)}
Attachment: {(attachments is null ? "N/A" : string.Join(";", attachments.Select(a => a.Name)))}
---------------------------------------------

{message}
                ").ConfigureAwait(false);
    }

    public async Task SendAsync<T>(Mailable<T> mailable)
    {
        await mailable.SendAsync(_renderer, this);
    }

    private static string CommaSeparated(IEnumerable<MailRecipient> recipients) =>
        (recipients ?? Enumerable.Empty<MailRecipient>())
            .Select(r => DisplayAddress(r))
            .CommaSeparated();

    private static string DisplayAddress(MailRecipient? recipient)
    {
        if (recipient == null)
            return string.Empty;
        else
            return $"{recipient.Name} <{recipient.Email}>";
    }
}