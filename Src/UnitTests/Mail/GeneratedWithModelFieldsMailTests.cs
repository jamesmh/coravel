using System.Linq;
using System.Threading.Tasks;
using Coravel.Mail.Mailers;
using UnitTests.Mail.Shared.Mailables;
using UnitTests.Mail.Shared.Models;
using Xunit;

namespace UnitTests.Mail.Shared
{
    public class GeneratedWithModelFieldsMailTests
    {
        [Fact]
        public async Task CheckSubjectAndEmailGeneratedFromFields()
        {
            var user = new TestUserWithFields("My Name", "autoassigned@test.com");

            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("Mailable With Model Fields", data.subject);
                Assert.Equal(user.Email, data.to.First());
            };

            await new AssertMailer(AssertMail).SendAsync(new MailableWithModelFields(user));
        }
    }
}