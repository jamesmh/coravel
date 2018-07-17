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

        public MailController(IMailer mailer){
            this._mailer = mailer;
        }

        public async Task<IActionResult> WithHtml() {

            UserModel user = new UserModel(){
                Email = "test@test.com",
                Name = "Coravel Test Person"
            };

            await new NewUserFromHtmlMailable(user).SendAsync(this._mailer);

            return Ok();
        }

        public async Task<IActionResult> RenderHtml() {

            UserModel user = new UserModel(){
                Email = "test@test.com",
                Name = "Coravel Test Person"
            };

            string message = await new NewUserFromHtmlMailable(user).Render(this._mailer);

            return Content(message, "text/html");
        }

       public async Task<IActionResult> WithView() {

            UserModel user = new UserModel(){
                Email = "test@test.com",
                Name = "Coravel Test Person"
            };

            await new NewUserFromViewMailable(user).SendAsync(this._mailer);

            return Ok();
        }

               public async Task<IActionResult> RenderView() {

            UserModel user = new UserModel(){
                Email = "test@test.com",
                Name = "Coravel Test Person"
            };

            string message = await new NewUserFromViewMailable(user).Render(this._mailer);

            return Content(message, "text/html");
        }
    }
}