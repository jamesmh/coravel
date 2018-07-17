using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coravel.Mail.Exceptions;
using Coravel.Mail.Interfaces;

namespace Coravel.Mail
{
    public abstract class Mailable : IViewMailable
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
        private object _viewData;

        public Mailable From(MailRecipient from)
        {
            this._from = from;
            return this;
        }

        public Mailable To(IEnumerable<string> to)
        {
            this._to = to;
            return this;
        }

        public Mailable Cc(IEnumerable<string> cc)
        {
            this._cc = cc;
            return this;
        }

        public Mailable Bcc(IEnumerable<string> bcc)
        {
            this._bcc = bcc;
            return this;
        }

        public Mailable ReplyTo(string replyTo)
        {
            this._replyTo = replyTo;
            return this;
        }

        public Mailable Subject(string subject)
        {
            this._subject = subject;
            return this;
        }

        public void Html(string html) => this._html = html;

        public IViewMailable WithViewData(object viewData)
        {
            this._viewData = viewData;
            return this;
        }

        public void View(string viewPath) => this._viewPath = viewPath;

        public abstract void Build();

        public async Task SendAsync(IMailer mailer)
        {
            string message = this.BuildMessage();
            await mailer.SendAsync(
                message,
                this._subject,
                this._to,
                this._replyTo,
                this._cc,
                this._bcc
            );
        }

        public string Render(){
            string message = this.BuildMessage();
            return message;
        }

        private string BuildMessage()
        {
            if (this._html != null)
            {
                return this._html;
            }

            if (this._viewPath != null)
            {
                object viewData = this.BuildViewData();
            }

            throw new NoMailRendererFound(NoRenderFoundMessage);
        }

        private object BuildViewData()
        {
            if (this._viewData == null)
            {
                return null;
            }

            Type type = this._viewData.GetType();

            var emailField = type.GetField("Email");

            if (emailField != null)
            {
                string viewDataEmailTo = (string)emailField.GetValue(this._viewData);
                this.To(new string[] { viewDataEmailTo });
            }

            return this._viewData;
        }
    }
}