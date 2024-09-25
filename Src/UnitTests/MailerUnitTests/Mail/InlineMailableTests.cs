using System.Linq;
using System.Threading.Tasks;
using Coravel.Mailer.Mail;
using Coravel.Mailer.Mail.Mailers;
using UnitTests.Mail.Shared.Models;
using Xunit;

namespace UnitTests.Mail
{
    public class InlineMailableTests
    {
        [Fact]
        public async Task MailableHasSubjectField()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("test", data.subject);
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableSubjectIsGeneratedFromMailableName()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("Inline", data.subject);
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From("from@test.com")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task CheckToAndNameFieldsAreGeneratedFromModel()
        {
            var user = new TestUser
            {
                Name = "My Name",
                Email = "autoassigned@test.com"
            };

            void AssertMail(AssertMailer.Data data)
            {
                var model = data.to.First();
                Assert.Equal(user.Email, model.Email);
                Assert.Equal(user.Name, model.Name);
            };

            await new AssertMailer(AssertMail).SendAsync(
                new InlineMailable()
                    .To(user)
                    .From("from@test.com")
                    .Html($"<html><body>Hi</body></html>")
            );
        }

        [Fact]
        public async Task MailableHasToField_OneAddress()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("to@test.com", data.to.First().Email);
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableHasToField_OneMailRecipient()
        {
            void AssertMail(AssertMailer.Data data)
            {
                var recipient = data.to.First();
                Assert.Equal("to@test.com", recipient.Email);
                Assert.Equal("My Name", recipient.Name);
            };

            var mail = new InlineMailable()
                .To(new MailRecipient("to@test.com", "My Name"))
                .From("from@test.com")
                .Subject("test")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableHasToField_MultiAddress()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal(3, data.to.Count());
            };

            var mail = new InlineMailable()
                .To(new string[] { "one@test.com", "two@test.com", "three@test.com" })
                .From("from@test.com")
                .Subject("test")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableHasToField_MultiMailRecipient()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal(3, data.to.Count());
            };

            var mail = new InlineMailable()
                .To(new MailRecipient[] {
                    new MailRecipient("one@test.com"),
                    new MailRecipient("two@test.com"),
                    new MailRecipient("three@test.com")
                })
                .From("from@test.com")
                .Subject("test")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableHasFromField_FromAddress()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("from@test.com", data.from.Email);
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableHasFromField_FromMailRecipient()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("from@test.com", data.from.Email);
                Assert.Equal("From", data.from.Name);
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From(new MailRecipient("from@test.com", "From"))
                .Subject("test")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableHasReplyToField_FromAddress()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("replyTo@test.com", data.replyTo.Email);
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .ReplyTo("replyTo@test.com")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableHasReplyToField_FromMailRecipient()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("replyTo@test.com", data.replyTo.Email);
                Assert.Equal("ReplyTo", data.replyTo.Name);
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .ReplyTo(new MailRecipient("replyTo@test.com", "ReplyTo"))
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableHasCcField_FromAddresses()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal(3, data.cc.Count());
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Cc(new string[] { "one@test.com", "two@test.com", "three@test.com" })
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableHasCcField_FromMailRecipients()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal(3, data.cc.Count());
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Cc(new MailRecipient[] {
                    new MailRecipient("one@test.com"),
                    new MailRecipient("two@test.com"),
                    new MailRecipient("three@test.com")
                })
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableHasBccField_FromAddresses()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal(3, data.bcc.Count());
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Bcc(new string[] { "one@test.com", "two@test.com", "three@test.com" })
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableHasBccField_FromMailRecipients()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal(3, data.bcc.Count());
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Bcc(new MailRecipient[] {
                    new MailRecipient("one@test.com"),
                    new MailRecipient("two@test.com"),
                    new MailRecipient("three@test.com")
                })
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }
        
        [Fact]
        public async Task MailableHasAttachments()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal(2, data.attachments.Count());
                Assert.True(data.attachments.Skip(1).Single().Name == "Attachment 2");
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Bcc(new MailRecipient[] {
                    new MailRecipient("one@test.com"),
                    new MailRecipient("two@test.com"),
                    new MailRecipient("three@test.com")
                })
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
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task MailableHasSender()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("sender@test.com", data.sender.Email);
            };

            var mail = new InlineMailable()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Sender("sender@test.com")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task using_inline_mailable_of_T_works_in_same_file_as_using_inline_mailable()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("sender@test.com", data.sender.Email);
            };

            // This generic type is used in the "View()" method. 
            // This test just makes sure that using this class and the non-generic version
            // in the same file works/compiles fine.
            var mail = new InlineMailable<TestUser>() 
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Sender("sender@test.com")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task inline_mailable_works_from_static_mailable_method()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("to@test.com", data.to.First().Email);
                Assert.Equal("from@test.com", data.from.Email);
                Assert.Equal("test", data.subject);
                Assert.Equal("<test></test>", data.message);
            };

            var mail = Mailable.AsInline()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }

        [Fact]
        public async Task inline_mailable_of_T_works_from_static_mailable_method()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("to@test.com", data.to.First().Email);
                Assert.Equal("from@test.com", data.from.Email);
                Assert.Equal("test", data.subject);
                Assert.Equal("<test></test>", data.message);
            };

            var mail = Mailable.AsInline<TestUser>()
                .To("to@test.com")
                .From("from@test.com")
                .Subject("test")
                .Html("<test></test>");

            await new AssertMailer(AssertMail).SendAsync(mail);
        }
    }
}