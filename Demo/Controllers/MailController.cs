using System;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Demo.Mailables;
using Demo.Models;

namespace Demo.Controllers
{
    public class MailController : Controller
    {
        private IMailer _mailer;

        public MailController(IMailer mailer)
        {
            this._mailer = mailer;
        }

        public async Task<IActionResult> WithHtml()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            await new WelcomeUserHtmlMail(user).SendAsync(this._mailer);

            return Ok();
        }

        public async Task<IActionResult> RenderHtml()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            string message = await new WelcomeUserHtmlMail(user).Render(this._mailer);

            return Content(message, "text/html");
        }

        public async Task<IActionResult> WithView()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            await new NewUserViewMail(user).SendAsync(this._mailer);

            return Ok();
        }

        public async Task<IActionResult> RenderView()
        {

            UserModel user = new UserModel()
            {
                Email = "FromUserModel@test.com",
                Name = "Coravel Test Person"
            };

            string message = await new NewUserViewMail(user).Render(this._mailer);

            return Content(message, "text/html");
        }
    }
}