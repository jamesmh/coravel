using System;
using System.Threading.Tasks;
using Coravel.Mailer.Mail;
using Coravel.Mailer.Mail.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TestMvcApp.Mailables;
using TestMvcApp.Models;

namespace TestMvcApp.Controllers
{
    [Route("Mail")]
    public class MailController : Controller
    {
        private IMailer _mailer;

        public MailController(IMailer mailer)
        {
            this._mailer = mailer;
        }

        [Route("WithHtml")]
        public async Task<IActionResult> WithHtml()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            await this._mailer.SendAsync(new WelcomeUserMailable(user));

            return Ok();
        }

        [Route("RenderHtml")]
        public async Task<IActionResult> RenderHtml()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            string message = await this._mailer.RenderAsync(new WelcomeUserMailable(user));

            return Content(message, "text/html");
        }

        [Route("WithView")]
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

        [Route("RenderView")]
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

        [Route("WithHtmlInlineMailable")]
        public async Task<IActionResult> WithHtmlInlineMailable()
        {
            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            await this._mailer.SendAsync(
                Mailable.AsInline()
                    .To(user)
                    .From("replyto@test.com")
                    .Html($"<html><body><h1>Welcome {user.Name}</h1></body></html>")
            );

            return Ok();
        }

        [Route("RenderHtmlInlineMailable")]
        public async Task<IActionResult> RenderHtmlInlineMailable()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            string message = await this._mailer.RenderAsync(
                Mailable.AsInline()
                    .To(user)
                    .From("replyto@test.com")
                    .Html($"<html><body><h1>Welcome {user.Name}</h1></body></html>")
            );

            return Content(message, "text/html");
        }

        [Route("WithHtmlInlineMailableOfT")]
        public async Task<IActionResult> WithHtmlInlineMailableOfT()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            await this._mailer.SendAsync(
                Mailable.AsInline<UserModel>()
                    .To(user)
                    .From("replyto@test.com")
                    .Html($"<html><body><h1>Welcome {user.Name}</h1></body></html>")
            );

            return Ok();
        }

        [Route("RenderHtmlInlineMailableOfT")]
        public async Task<IActionResult> RenderHtmlInlineMailableOfT()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            string message = await this._mailer.RenderAsync(
                Mailable.AsInline<UserModel>()
                    .To(user)
                    .From("replyto@test.com")
                    .Html($"<html><body><h1>Welcome {user.Name}</h1></body></html>")
            );

            return Content(message, "text/html");
        }
    }
}