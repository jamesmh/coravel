using System.Linq;
using System.Threading.Tasks;
using Coravel.Mailer.Mail;
using Coravel.Mailer.Mail.Mailers;
using UnitTests.Mail.Shared.Mailables;
using UnitTests.Mail.Shared.Models;
using Xunit;

namespace UnitTests.Mail;

public class GeneralMailTests
{
    [Fact]
    public async Task MailableHasSubjectField()
    {
        void AssertMail(AssertMailer.Data data)
        {
            Assert.Equal("test", data.Subject);
        }

        var mail = new GenericHtmlMailable()
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
            Assert.Equal("Generic Html", data.Subject);
        }

        var mail = new GenericHtmlMailable()
            .Subject("Generic Html")
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
            var model = data.To.First();
            Assert.Equal(user.Email, model.Email);
            Assert.Equal(user.Name, model.Name);
        }

        await new AssertMailer(AssertMail).SendAsync(new MailableWithModelProperties(user));
    }

    [Fact]
    public async Task MailableHasToField_OneAddress()
    {
        void AssertMail(AssertMailer.Data data)
        {
            Assert.Equal("to@test.com", data.To.First().Email);
        }

        var mail = new GenericHtmlMailable()
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
            var recipient = data.To.First();
            Assert.Equal("to@test.com", recipient.Email);
            Assert.Equal("My Name", recipient.Name);
        }

        var mail = new GenericHtmlMailable()
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
            Assert.Equal(3, data.To.Count());
        }

        var mail = new GenericHtmlMailable()
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
            Assert.Equal(3, data.To.Count());
        }

        var mail = new GenericHtmlMailable()
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
            Assert.Equal("from@test.com", data.From?.Email);
        }

        var mail = new GenericHtmlMailable()
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
            Assert.Equal("from@test.com", data.From?.Email);
            Assert.Equal("From", data.From?.Name);
        }

        var mail = new GenericHtmlMailable()
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
            Assert.Equal("replyTo@test.com", data.ReplyTo?.Email);
        }

        var mail = new GenericHtmlMailable()
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
            Assert.Equal("replyTo@test.com", data.ReplyTo?.Email);
            Assert.Equal("ReplyTo", data.ReplyTo?.Name);
        }

        var mail = new GenericHtmlMailable()
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
            Assert.Equal(3, data.Cc.Count());
        }

        var mail = new GenericHtmlMailable()
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
            Assert.Equal(3, data.Cc.Count());
        }

        var mail = new GenericHtmlMailable()
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
            Assert.Equal(3, data.Bcc.Count());
        }

        var mail = new GenericHtmlMailable()
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
            Assert.Equal(3, data.Bcc.Count());
        }

        var mail = new GenericHtmlMailable()
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
            Assert.Equal(2, data.Attachments?.Count());
            Assert.True(data.Attachments?.Skip(1).Single().Name == "Attachment 2");
        }

        var mail = new GenericHtmlMailable()
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
                Bytes = System.Array.Empty<byte>(),
                Name =  "Attachment 1"
            })
            .Attach(new Attachment
            {
                Bytes = System.Array.Empty<byte>(),
                Name =  "Attachment 2"
            })
            .Html("<test></test>");

        await new AssertMailer(AssertMail).SendAsync(mail);
    }
}