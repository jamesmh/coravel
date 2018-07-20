using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Coravel.Mail.Exceptions;
using Coravel.Mail.Helpers;
using Coravel.Mail.Interfaces;

namespace Coravel.Mail
{
    public class Mailable<T>
    {
        private static readonly string NoRenderFoundMessage = "Please use one of the available methods for specifying how to render your mail (e.g. Html() or View()";

        /// <summary>
        /// Who the email is from.
        /// </summary>
        private MailRecipient _from;

        /// <summary>
        /// Recipients of the message.
        /// </summary>
        private IEnumerable<string> _to;

        /// <summary>
        /// cc recipients of the message.
        /// </summary>
        private IEnumerable<string> _cc;

        /// <summary>
        /// bcc recipients of the message.
        /// </summary>
        private IEnumerable<string> _bcc;

        /// <summary>
        /// Who the recipients should reply to.
        /// </summary>
        private string _replyTo;

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

        /// <summary>
        /// View data to pass to the view to render.
        /// </summary>
        private T _viewModel;

        public Mailable<T> From(string email, string name)
        {
            this._from = new MailRecipient
            {
                Email = email,
                Name = name
            };
            return this;
        }

        public Mailable<T> From(string email) => this.From(email, null);

        public Mailable<T> To(IEnumerable<string> to)
        {
            this._to = to;
            return this;
        }

        public Mailable<T> To(string to) => this.To(new string[] { to });

        public Mailable<T> To(object mailToModel)
        {
            this._mailToModel = mailToModel;
            return this;
        }

        public Mailable<T> Cc(IEnumerable<string> cc)
        {
            this._cc = cc;
            return this;
        }

        public Mailable<T> Bcc(IEnumerable<string> bcc)
        {
            this._bcc = bcc;
            return this;
        }

        public Mailable<T> ReplyTo(string replyTo)
        {
            this._replyTo = replyTo;
            return this;
        }

        public Mailable<T> Subject(string subject)
        {
            this._subject = subject;
            return this;
        }

        public void Html(string html) => this._html = html;

        public void View(string viewPath, T viewModel)
        {
            this._viewModel = viewModel;
            this._viewPath = viewPath;
        }

        public virtual void Build() { }

        internal async Task SendAsync(IRazorRenderer renderer, IMailer mailer)
        {
            this.Build();

            string message = await this.BuildMessage(renderer, mailer).ConfigureAwait(false);

            await mailer.SendAsync(
                message,
                this._subject,
                this._to,
                this._from.Email,
                this._replyTo,
                this._cc,
                this._bcc
            ).ConfigureAwait(false);
        }

        internal async Task<string> Render(IRazorRenderer renderer, IMailer mailer)
        {
            this.Build();
            return await this.BuildMessage(renderer, mailer).ConfigureAwait(false);
        }

        private async Task<string> BuildMessage(IRazorRenderer renderer, IMailer mailer)
        {
            this.BindViewModelToFields();

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

        private void BindViewModelToFields()
        {
            if (this._mailToModel != null)
            {
                BindEmailField();
                BindSubjectField();
            }
        }

        private void BindEmailField()
        {
            Type modelType = this._mailToModel.GetType();
            MemberInfo emailMember = modelType.GetProperty("Email") as MemberInfo ?? modelType.GetField("Email");

            object emailTo = null;
            if (emailMember == null)
            {
                return;
            }
            else if (emailMember is PropertyInfo prop)
            {
                emailTo = prop.GetValue(this._mailToModel);
            }
            else if (emailMember is FieldInfo field)
            {
                emailTo = field.GetValue(this._mailToModel);
            }

            if (emailTo is IEnumerable<string> enumerableTo)
            {
                this.To(enumerableTo);
            }
            else if (emailTo is string stringTo)
            {
                this.To(stringTo);
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