using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Mailer.Mail;
using Coravel.Mailer.Mail.Mailers;
using Coravel.Mailer.Mail.Renderers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using UnitTests.Mail.Shared.Mailables;
using Xunit;

namespace UnitTests.Mail
{
    public class SmtpMailerTests
    {
        [Fact]
        public async Task SmtpMailerRenderSucessful()
        {
            var renderer = RazorRendererFactory.MakeInstance(new ConfigurationBuilder().Build());
            var mailer = new SmtpMailer(renderer, "dummy", 1, "dummy", "dummy");

            string message = await mailer.RenderAsync(
                new GenericHtmlMailable()
                    .Subject("test")
                    .From("from@test.com")
                    .To("to@test.com")
                    .Html("<html></html>")
            );

            Assert.Equal("<html></html>", message);
        }
    }
}