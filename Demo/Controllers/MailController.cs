using System;
using System.Threading.Tasks;
using Coravel.Mailer.Mail.Interfaces;
using Coravel.Mailer.Mail;
using Microsoft.AspNetCore.Mvc;
using Demo.Mailables;
using Demo.Models;
using Coravel.Queuing.Interfaces;

namespace Demo.Controllers
{
    public class MailController : Controller
    {
        private IMailer _mailer;
        private IQueue _queue;

        public MailController(IMailer mailer, IQueue queue)
        {
            this._mailer = mailer;
            this._queue = queue;
        }

        public async Task<IActionResult> WithHtml()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            await this._mailer.SendAsync(new WelcomeUserHtmlMail(user));

            return Ok();
        }

        public async Task<IActionResult> RenderHtml()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            string message = await this._mailer.RenderAsync(new WelcomeUserHtmlMail(user));

            return Content(message, "text/html");
        }

        public async Task<IActionResult> WithView()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            await this._mailer.SendAsync(new NewUserViewMail(user));

            return Ok();
        }

        public async Task<IActionResult> RenderView()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            string message = await this._mailer.RenderAsync(new NewUserViewMail(user));

            return Content(message, "text/html");
        }

        public async Task<IActionResult> RenderViewWithInlineMailable()
        {
            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            string message = await this._mailer.RenderAsync(
                Mailable.AsInline<UserModel>()
                    .To(user)
                    .From("from@test.com")
                    .View("~/Views/Mail/NewUser.cshtml", user)
            );

            return Content(message, "text/html");
        }

        public async Task<IActionResult> WithHtmlAndPlainText()
        {
            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            await this._mailer.SendAsync(
                Mailable.AsInline<UserModel>()
                    .To(user)
                    .From("from@test.com")
                    .View("~/Views/Mail/NewUser.cshtml", user)
                    .Text("This is a plain text version of the email!")
            );

            return Ok();
        }

        public IActionResult QueueMail()
        {
            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            async Task Send() =>
                await this._mailer.SendAsync(new NewUserViewMail(user));

            this._queue.QueueAsyncTask(Send);

            return Content("Mail was queued!");
        }
    }
}