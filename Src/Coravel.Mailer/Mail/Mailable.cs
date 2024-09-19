using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Mailer.Mail.Exceptions;
using Coravel.Mailer.Mail.Helpers;
using Coravel.Mailer.Mail.Interfaces;
using Coravel.Mailer.Mail.Renderers;

namespace Coravel.Mailer.Mail
{
    public class Mailable<T>
    {
        private static readonly string NoRenderFoundMessage = "Please use one of the available methods for specifying how to render your mail (e.g. Html() or View())";

        /// <summary>
        /// Who the email is from.
        /// </summary>
        private MailRecipient _from;

        /// <summary>
        /// The email sender.
        /// </summary>
        private MailRecipient _sender;

        /// <summary>
        /// Recipients of the message.
        /// </summary>
        private IEnumerable<MailRecipient> _to;

        /// <summary>
        /// cc recipients of the message.
        /// </summary>
        private IEnumerable<MailRecipient> _cc;

        /// <summary>
        /// bcc recipients of the message.
        /// </summary>
        private IEnumerable<MailRecipient> _bcc;

        /// <summary>
        /// Who the recipients should reply to.
        /// </summary>
        private MailRecipient _replyTo;

        /// <summary>
        /// Mail's subject.
        /// </summary>
        private string _subject;

        /// <summary>
        /// Raw HTML to use as email message.
        /// </summary>
        private string _html;

        /// <summary>
        /// The MVC view to use to generate the message.
        /// </summary>
        private string _viewPath;

        /// <summary>
        /// Model that we want to mail to.
        /// </summary>
        private object _mailToModel;

        private List<Attachment> _attachments;

        /// <summary>
        /// View data to pass to the view to render.
        /// </summary>
        private T _viewModel;

        public Mailable<T> From(MailRecipient recipient)
        {
            this._from = recipient;
            return this;
        }

        public Mailable<T> From(string email) =>
            this.From(new MailRecipient(email));

        public Mailable<T> Sender(MailRecipient recipient)
        {
            this._sender = recipient;
            return this;
        }

        public Mailable<T> Sender(string email) =>
            this.Sender(new MailRecipient(email));

        public Mailable<T> To(IEnumerable<MailRecipient> recipients)
        {
            this._to = recipients;
            return this;
        }

        public Mailable<T> To(MailRecipient recipient) =>
            this.To(new MailRecipient[] { recipient });

        public Mailable<T> To(IEnumerable<string> addresses) =>
            this.To(addresses.Select(address => new MailRecipient(address)));

        public Mailable<T> To(string email) =>
            this.To(new MailRecipient(email));

        public Mailable<T> To(object mailToModel)
        {
            this._mailToModel = mailToModel;
            return this;
        }

        public Mailable<T> Cc(IEnumerable<MailRecipient> recipients)
        {
            this._cc = recipients;
            return this;
        }

        public Mailable<T> Cc(IEnumerable<string> addresses) =>
            this.Cc(addresses.Select(address => new MailRecipient(address)));

        public Mailable<T> Bcc(IEnumerable<MailRecipient> recipients)
        {
            this._bcc = recipients;
            return this;
        }

        public Mailable<T> Bcc(IEnumerable<string> addresses) =>
            this.Bcc(addresses.Select(address => new MailRecipient(address)));

        public Mailable<T> ReplyTo(MailRecipient replyTo)
        {
            this._replyTo = replyTo;
            return this;
        }

        public Mailable<T> ReplyTo(string address)
        {
            this._replyTo = new MailRecipient(address);
            return this;
        }

        public Mailable<T> Subject(string subject)
        {
            this._subject = subject;
            return this;
        }

        public Mailable<T> Attach(Attachment attachment)
        {
            if(this._attachments is null)
            {
                this._attachments = new List<Attachment>();
            }
            this._attachments.Add(attachment);
            return this;
        }

        public Mailable<T> Html(string html)
        {
            this._html = html;
            return this;
        }

        public Mailable<T> View(string viewPath, T viewModel)
        {
            this._viewModel = viewModel;
            this._viewPath = viewPath;
            return this;
        }

        public Mailable<T> View(string viewPath)
        {
            this.View(viewPath, default(T));
            return this;
        }

        public virtual void Build() { }

        internal async Task SendAsync(RazorRenderer renderer, IMailer mailer)
        {
            this.Build();

            string message = await this.BuildMessage(renderer, mailer).ConfigureAwait(false);

            await mailer.SendAsync(
                message,
                this._subject,
                this._to,
                this._from,
                this._replyTo,
                this._cc,
                this._bcc,
                this._attachments,
                sender: this._sender
            ).ConfigureAwait(false);
        }

        internal async Task<string> RenderAsync(RazorRenderer renderer, IMailer mailer)
        {
            this.Build();
            return await this.BuildMessage(renderer, mailer).ConfigureAwait(false);
        }

        private async Task<string> BuildMessage(RazorRenderer renderer, IMailer mailer)
        {
            this.BindDynamicProperties();

            if (this._html != null)
            {
                return this._html;
            }

            if (this._viewPath != null)
            {
                return await renderer
                    .RenderViewToStringAsync<T>(this._viewPath, this._viewModel)
                    .ConfigureAwait(false);
            }

            throw new NoMailRendererFound(NoRenderFoundMessage);
        }

        private void BindDynamicProperties()
        {
            if (this.HasMailToModel())
            {
                this.BindEmailField();
            }

            if (this.HasNoSubject())
            {
                this.BindSubjectField();
            }
        }

        private bool HasNoSubject() => this._subject == null;

        private bool HasMailToModel() => this._mailToModel != null;

        private void BindEmailField()
        {
            object propEmail = this._mailToModel.GetPropOrFieldValue("Email");
            object propName = this._mailToModel.GetPropOrFieldValue("Name");

            if (propEmail is string address)
            {
                if (propName is string name)
                {
                    this.To(new MailRecipient(address, name));
                }
                else
                {
                    this.To(address);
                }
            }
        }

        private void BindSubjectField()
        {
            if (this._subject == null)
            {
                string classNameOfThisMailable = this.GetType().Name;

                this._subject = classNameOfThisMailable
                    .ToSnakeCase()
                    .RemoveLastOccuranceOfWord("Mailable");
            }
        }
    }
}