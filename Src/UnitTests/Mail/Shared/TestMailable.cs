using Coravel.Mail;

namespace UnitTests.Mail.Shared
{
    public class TestMailable : Mailable<string>
    {
        public override void Build()
        {
            this.To("to@test.com")
                .From("from@test.com")
                .Html("<html><body>hi!</body></html>");
        }
    }
}