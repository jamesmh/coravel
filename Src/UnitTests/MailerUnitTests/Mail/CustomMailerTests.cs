using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Mailer.Mail;
using Coravel.Mailer.Mail.Mailers;
using Coravel.Mailer.Mail.Renderers;
using Microsoft.Extensions.Configuration;
using UnitTests.Mail.Shared.Mailables;
using Xunit;

namespace UnitTests.Mail
{
    public class CustomMailerTests
    {
        [Fact]
        public async Task CustomMailerSucessful()
        {
            async Task SendMailCustom(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc)
            {
                Assert.Equal("test", subject);
                Assert.Equal("from@test.com", from.Email);
                Assert.Equal("to@test.com", to.First().Email);
                await Task.CompletedTask;
            };

            var mailer = new CustomMailer(
                null, // We aren't rendering anything, so it's null.
                SendMailCustom,
                null
            );

            await mailer.SendAsync(
                new GenericHtmlMailable()
                    .Subject("test")
                    .From("from@test.com")
                    .To("to@test.com")
                    .Html("test")
            );
        }

        [Fact]
        public async Task CustomMailer_GlobalFrom()
        {
            async Task SendMailCustom(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc)
            {
                Assert.Equal("global@test.com", from.Email);
                Assert.Equal("Global", from.Name);
                await Task.CompletedTask;
            };

            var mailer = new CustomMailer(
                null, // We aren't rendering anything, so it's null.
                SendMailCustom,
                new MailRecipient("global@test.com", "Global")
            );

            await mailer.SendAsync(
                new GenericHtmlMailable()
                    .Subject("test")
                    .From("from@test.com") // Shoudl be ignored due to global "from"
                    .To("to@test.com")
                    .Html("test")
            );
        }

        [Fact]
        public async Task CustomMailer_Render()
        {
            async Task SendMailCustom(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc)
            {
                await Task.CompletedTask;
            };

            var renderer = RazorRendererFactory.MakeInstance(new ConfigurationBuilder().Build());

            var mailer = new CustomMailer(
                renderer,
                SendMailCustom
            );

            var htmlMessage = await mailer.RenderAsync(
                new GenericHtmlMailable()
                    .Subject("test")
                    .From("from@test.com") // Shoudl be ignored due to global "from"
                    .To("to@test.com")
                    .Html("<html></html>")
            );

            Assert.Equal("<html></html>", htmlMessage);
        }
    }
}