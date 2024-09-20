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
            async Task SendMailCustom(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments = null, MailRecipient sender = null)
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
            async Task SendMailCustom(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments = null, MailRecipient sender = null)
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
                    //.From("from@test.com")
                    .To("to@test.com")
                    .Html("test")
            );
        }

        [Fact]
        public async Task CustomMailer_GlobalFromIsOverridedByFrom()
        {
            async Task SendMailCustom(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments = null, MailRecipient sender = null)
            {
                Assert.Equal("from@test.com", from.Email);
                Assert.Null(from.Name);
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
                    .From("from@test.com") // Should override the global from.
                    .To("to@test.com")
                    .Html("test")
            );
        }

        [Fact]
        public async Task CustomMailer_Render()
        {
            async Task SendMailCustom(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments = null, MailRecipient sender = null)
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
                    .From("from@test.com") // Should be ignored due to global "from"
                    .To("to@test.com")
                    .Html("<html></html>")
            );

            Assert.Equal("<html></html>", htmlMessage);
        }
        
        [Fact]
        public async Task CustomMailerHasAttachments()
        {
            async Task SendMailCustom(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments = null, MailRecipient sender = null)
            {
                Assert.Equal(2, attachments.Count());
                Assert.Equal("Attachment 2", attachments.Skip(1).Single().Name);
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
                    .Attach(new Attachment
                    {
                        Bytes = new byte[] { },
                        Name =  "Attachment 1"
                    })
                    .Attach(new Attachment
                    {
                        Bytes = new byte[] { },
                        Name =  "Attachment 2"
                    })
            );
        }
    }
}