namespace UnitTests.Mail.Shared.Models
{
    public class TestUserWithFields
    {
        public TestUserWithFields(string name, string email)
        {
            this.Name = name;
            this.Email = email;
        }
        public string Name;
        public string Email;
    }
}