using Coravel.Mail;
using Coravel.Mail.Interfaces;
using Coravel.Mail.Mailers;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel
{
    /// <summary>
    /// IServiceCollection extensions for registering Coravel's Mailer.
    /// </summary>
    public static class MailServiceRegistration
    {
        /// <summary>
        /// Register Coravel's mailer using the LogMailer - which sends mail to a file.
        /// Useful for testing.
        /// </summary>
        /// <param name="services"></param>
        public static void AddFileLogMailer(this IServiceCollection services)
        {
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddScoped<IMailer, FileLogMailer>();
        }
    }
}