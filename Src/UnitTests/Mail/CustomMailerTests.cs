using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Mail;
using Coravel.Mail.Interfaces;
using Coravel.Mail.Mailers;
using Coravel.Mail.Renderers;
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
                SendMailCustom
            );

            await mailer.SendAsync(
                new GenericHtmlMailable()
                    .Subject("test")
                    .From("from@test.com")
                    .To("to@test.com")
                    .Html("test")
            );
        }
    }
}