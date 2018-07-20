using System.Linq;
using System.Threading.Tasks;
using UnitTests.Mail.Shared.Mailables;
using UnitTests.Mail.Shared.Mailers;
using UnitTests.Mail.Shared.Models;
using Xunit;

namespace UnitTests.Mail.Shared
{
    public class GeneratedFieldsMailTests
    {
        [Fact]
        public async Task CheckSubjectIsGeneratedFromMailableName()
        {
            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal("Auto To And Subject", data.subject);
            };

            var user = new TestUser
            {
                Name = "My Name",
                Email = "autoassigned@test.com"
            };

            await new AssertMailer(AssertMail).SendAsync(new AutoToAndSubjectMailable(user));
        }

        [Fact]
        public async Task CheckToFieldIsGeneratedFromMailableName()
        {
            var user = new TestUser
            {
                Name = "My Name",
                Email = "autoassigned@test.com"
            };

            void AssertMail(AssertMailer.Data data)
            {
                Assert.Equal(user.Email, data.to.First());
            };

            await new AssertMailer(AssertMail).SendAsync(new AutoToAndSubjectMailable(user));
        }
    }
}