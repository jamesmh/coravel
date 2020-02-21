using System;
using System.Collections.Generic;
using System.Net.Security;
using Coravel.Mailer.Mail;
using Coravel.Mailer.Mail.Interfaces;
using Coravel.Mailer.Mail.Mailers;
using Coravel.Mailer.Mail.Renderers;
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
        public static IServiceCollection AddMailer(this IServiceCollection services, IConfiguration config)
        {
            string mailerType = config.GetValue<string>("Coravel:Mail:Driver", "FileLog");

            var strategies = new Dictionary<string, Action>();
            strategies.Add("SMTP", () => AddSmtpMailer(services, config));
            strategies.Add("FILELOG", () => AddFileLogMailer(services, config));
            strategies[mailerType.ToUpper()].Invoke();
            return services;
        }

        /// <summary>
        /// Register Coravel's mailer using the File Log Mailer - which sends mail to a file.
        /// Useful for testing.
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddFileLogMailer(this IServiceCollection services, IConfiguration config)
        {
            var globalFrom = GetGlobalFromRecipient(config);
            RazorRenderer renderer = RazorRendererFactory.MakeInstance(config);
            var mailer = new FileLogMailer(renderer, globalFrom);
            services.AddSingleton<IMailer>(mailer);
            return services;
        }

        /// <summary>
        /// Register Coravel's mailer using the Smtp Mailer.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="certCallback"></param>
        public static IServiceCollection AddSmtpMailer(this IServiceCollection services, IConfiguration config, RemoteCertificateValidationCallback certCallback)
        {
            var globalFrom = GetGlobalFromRecipient(config);
            RazorRenderer renderer = RazorRendererFactory.MakeInstance(config);
            IMailer mailer = new SmtpMailer(
                renderer,
                config.GetValue<string>("Coravel:Mail:Host", ""),
                config.GetValue<int>("Coravel:Mail:Port", 0),
                config.GetValue<string>("Coravel:Mail:Username", null),
                config.GetValue<string>("Coravel:Mail:Password", null),
                globalFrom,
                certCallback
            );
            services.AddSingleton<IMailer>(mailer);
            return services;
        }

        /// <summary>
        /// Register Coravel's mailer using the Smtp Mailer.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        public static IServiceCollection AddSmtpMailer(this IServiceCollection services, IConfiguration config)
        {
            AddSmtpMailer(services, config, null);
            return services;
        }

        /// <summary>
        /// Register Coravel's mailer using the Custom Mailer.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="sendMailAsync"></param>
        public static IServiceCollection AddCustomMailer(this IServiceCollection services, IConfiguration config, CustomMailer.SendAsyncFunc sendMailAsync)
        {
            var globalFrom = GetGlobalFromRecipient(config);
            RazorRenderer renderer = RazorRendererFactory.MakeInstance(config);
            var mailer = new CustomMailer(renderer, sendMailAsync, globalFrom);
            services.AddSingleton<IMailer>(mailer);
            return services;
        }

        private static MailRecipient GetGlobalFromRecipient(IConfiguration config)
        {
            string globalFromAddress = config.GetValue<string>("Coravel:Mail:From:Address", null);
            string globalFromName = config.GetValue<string>("Coravel:Mail:From:Name", null);

            if (globalFromAddress != null)
            {
                return new MailRecipient(globalFromAddress, globalFromName);
            }
            else
            {
                return null;
            }
        }
    }
}