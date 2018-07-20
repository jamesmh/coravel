using System;
using System.Collections.Generic;
using System.Net.Security;
using Coravel.Mail.Interfaces;
using Coravel.Mail.Mailers;
using Coravel.Mail.Renderers;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel
{
    /// <summary>
    /// IServiceCollection extensions for registering Coravel's Mailer.
    /// </summary>
    public static class MailServiceRegistration
    {
        /// <summary>
        /// Register Coravel's mailer using the IConfiguration to provide all configuration details.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static void AddMailer(this IServiceCollection services, IConfiguration config){
            string mailerType = config.GetValue<string>("Coravel:Mail:Driver", "FileLog");

            var strategies = new Dictionary<string, Action>();
            strategies.Add("SMTP", () => AddSmtpMailer(services, config));
            strategies.Add("FILELOG", () => AddFileLogMailer(services));

            strategies[mailerType.ToUpper()].Invoke();           
        }

        /// <summary>
        /// Register Coravel's mailer using the File Log Mailer - which sends mail to a file.
        /// Useful for testing.
        /// </summary>
        /// <param name="services"></param>
        public static void AddFileLogMailer(this IServiceCollection services)
        {
            services.AddScoped<IRazorRenderer, RazorRenderer>();
            services.AddScoped<IMailer, FileLogMailer>();
        }

        /// <summary>
        /// Register Coravel's mailer using the Smtp Mailer.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="certCallback"></param>
        public static void AddSmtpMailer(this IServiceCollection services, IConfiguration config, RemoteCertificateValidationCallback certCallback) {
            services.AddScoped<IRazorRenderer, RazorRenderer>();
            services.AddScoped<IMailer>(p =>
                new SmtpMailer(
                    p.GetService<IRazorRenderer>(),
                    config.GetValue<string>("Coravel:Mail:Host", ""),
                    config.GetValue<int>("Coravel:Mail:Port", 0),
                    config.GetValue<string>("Coravel:Mail:Username", ""),
                    config.GetValue<string>("Coravel:Mail:Password", ""),
                    certCallback
                )
            );
        }

        /// <summary>
        /// Register Coravel's mailer using the Smtp Mailer.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
         public static void AddSmtpMailer(this IServiceCollection services, IConfiguration config) {
             AddSmtpMailer(services, config, null);
         }
    }
}