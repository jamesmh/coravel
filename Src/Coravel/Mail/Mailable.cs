using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mail.Exceptions;
using Coravel.Mail.Interfaces;

namespace Coravel.Mail
{
    public abstract class Mailable<T>
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
        /// View data to pass to the view to render.
        /// </summary>
        private T _viewData;

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

        public void View(string viewPath, T viewData)
        {
            this._viewPath = viewPath;
            this._viewData = viewData;            
        }

        public abstract void Build();

        public async Task SendAsync(IMailer mailer)
        {
            this.Build();

            string message = await this.BuildMessage(mailer);

            await mailer.SendAsync(
                message,
                this._subject,
                this._to,
                this._replyTo,
                this._cc,
                this._bcc
            );
        }

        public async Task<string> Render(IMailer mailer)
        {
            this.Build();
            return await this.BuildMessage(mailer);
        }

        private async Task<string> BuildMessage(IMailer mailer)
        {
            if (this._html != null)
            {
                return this._html;
            }

            if (this._viewPath != null)
            {
                T viewData = this.BuildViewData();
                return await mailer.GetViewRenderer().RenderViewToStringAsync<T>(this._viewPath, viewData);
            }

            throw new NoMailRendererFound(NoRenderFoundMessage);
        }

        private T BuildViewData()
        {
            if (this._viewData == null)
            {
                return default(T);
            }

            Type type = this._viewData.GetType();

            var emailField = type.GetField("Email");

            if (emailField != null)
            {
                string viewDataEmailTo = (string) emailField.GetValue(this._viewData);
                this.To(new string[] { viewDataEmailTo });
            }

            return this._viewData;
        }
    }
}