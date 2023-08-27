namespace UnitTests.Mail.Shared.Models;

public class TestUserWithFields
{
    public TestUserWithFields(string name, string email)
    {
        Name = name;
        Email = email;
    }
    public string Name;
    public string Email;
}