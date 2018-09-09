using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Mailer.Mail.Interfaces;
using Demo.Mailables;
using Demo.Models;

namespace Demo.Invocables
{
    public class SendNightlyReportsEmailJob : IInvocable
    {
        private IMailer _mailer;
        public SendNightlyReportsEmailJob(IMailer mailer)
        {
            this._mailer = mailer;
        }

        public async Task Invoke()
        {
            await Task.Delay(10000);

            // You could grab multiple users from a DB query ;)
            var mailable = new NightlyReportMailable(new UserModel
            {
                Name = "Coravel is awesome!",
                Email = "test@test.com"
            });
            await this._mailer.SendAsync(mailable);
            Console.WriteLine($"NightlyReportMailable was sent at {DateTime.UtcNow}.");
        }
    }
}