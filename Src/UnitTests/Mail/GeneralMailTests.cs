using System.Linq;
using System.Threading.Tasks;
using Coravel.Mail;
using Coravel.Mail.Mailers;
using UnitTests.Mail.Shared.Mailables;
using UnitTests.Mail.Shared.Models;
using Xunit;

namespace UnitTests.Mail
{
    public class GeneralMailTests
    {
        [Fact]
        public async Task MailableHasSubjectField()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("test", data.subject);
            };

            await new AssertMailer(AssertMail).SendAsync(new GenericHtmlMailable());
        }

        [Fact]
        public async Task MailableHasToField()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("to@test.com", data.to.First());
            };

            await new AssertMailer(AssertMail).SendAsync(new GenericHtmlMailable());
        }

        [Fact]
        public async Task MailableHasFromField()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("from@test.com", data.from);
            };

            await new AssertMailer(AssertMail).SendAsync(new GenericHtmlMailable());
        }

        [Fact]
        public async Task MailableHasReplyToField()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("replyTo@test.com", data.replyTo);
            };

            await new AssertMailer(AssertMail).SendAsync(new GenericHtmlMailable());
        }

        [Fact]
        public async Task MailableHasCcField()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("cc@test.com", data.cc.First());
            };

            await new AssertMailer(AssertMail).SendAsync(new GenericHtmlMailable());
        }

        [Fact]
        public async Task MailableHasBccField()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("bcc@test.com", data.bcc.First());
            };

            await new AssertMailer(AssertMail).SendAsync(new GenericHtmlMailable());
        }
    }
}