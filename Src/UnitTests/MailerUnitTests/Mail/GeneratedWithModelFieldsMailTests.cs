using System.Linq;
using System.Threading.Tasks;
using Coravel.Mailer.Mail.Mailers;
using UnitTests.Mail.Shared.Mailables;
using UnitTests.Mail.Shared.Models;
using Xunit;

namespace UnitTests.Mail.Shared;

public class GeneratedWithModelFieldsMailTests
{
    [Fact]
    public async Task CheckSubject_Email_Name_GeneratedFromFields()
    {
        var user = new TestUserWithFields("My Name", "autoassigned@test.com");

        void AssertMail(AssertMailer.Data data)
        {
            Assert.Equal(user.Email, data.To.First().Email);
            Assert.Equal(user.Name, data.To.First().Name);
        }

        await new AssertMailer(AssertMail).SendAsync(new MailableWithModelFields(user));
    }
}