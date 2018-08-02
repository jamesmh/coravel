using System;
using System.Threading.Tasks;
using Coravel.Mail.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TestMvcApp.Mailables;
using TestMvcApp.Models;

namespace TestMvcApp.Controllers
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
    }
}