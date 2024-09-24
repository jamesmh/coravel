using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Mailer.Mail;
using Coravel.Mailer.Mail.Mailers;
using Coravel.Mailer.Mail.Renderers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Mail.Shared.Mailables;
using Xunit;

namespace UnitTests.Mail
{
    public class CanSendMailWrapperTests
    {
        public class CustomAssertMailer : ICanSendMail
        {
            private Action<string, string, IEnumerable<MailRecipient>, MailRecipient, MailRecipient, IEnumerable<MailRecipient>, IEnumerable<MailRecipient>, IEnumerable<Attachment>, MailRecipient> _assert;

            public CustomAssertMailer(Action<string, string, IEnumerable<MailRecipient>, MailRecipient, MailRecipient, IEnumerable<MailRecipient>, IEnumerable<MailRecipient>, IEnumerable<Attachment>, MailRecipient> assert)
            {
                this._assert = assert;
            }

            public Task SendAsync(string message, string subject, IEnumerable<MailRecipient> to, MailRecipient from, MailRecipient replyTo, IEnumerable<MailRecipient> cc, IEnumerable<MailRecipient> bcc, IEnumerable<Attachment> attachments = null, MailRecipient sender = null)
            {
                this._assert(message, subject, to, from, replyTo, cc, bcc, attachments, sender);
                return Task.CompletedTask;
            }
        }


        [Fact]
        public async Task when_sending_generic_mail_it_sends()
        {
            var services = new ServiceCollection();
            services.AddScoped<Action<string, string, IEnumerable<MailRecipient>, MailRecipient, MailRecipient, IEnumerable<MailRecipient>, IEnumerable<MailRecipient>, IEnumerable<Attachment>, MailRecipient>>(p =>
                (message, subject, to, from, replyTo, cc, bcc, attachments, sender) =>
                { 
                    Assert.Equal("test", message);
                    Assert.Equal("from@test.com", from.Email);
                    Assert.Equal("from@test.com", from.Email);
                    Assert.Equal("to@test.com", to.First().Email);
                    Assert.Equal("test", message);
                });

            services.AddScoped<CustomAssertMailer>();
            var provider = services.BuildServiceProvider();

            var mailer = new CanSendMailWrapper<CustomAssertMailer>(
                RazorRendererFactory.MakeInstance(new ConfigurationBuilder().Build()),
                provider.GetRequiredService<IServiceScopeFactory>(), 
                null
            );

            await mailer.SendAsync(
                new GenericHtmlMailable()
                    .Subject("test")
                    .From("from@test.com")
                    .To("to@test.com")
                    .Html("test")
            );
        }

        [Fact]
        public async Task when_using_global_from_when_from_not_defined_it_uses_global_value()
        {
            var services = new ServiceCollection();
            services.AddScoped<Action<string, string, IEnumerable<MailRecipient>, MailRecipient, MailRecipient, IEnumerable<MailRecipient>, IEnumerable<MailRecipient>, IEnumerable<Attachment>, MailRecipient>>(p =>
                (message, subject, to, from, replyTo, cc, bcc, attachments, sender) =>
                { 
                    Assert.Equal("test", message);
                    Assert.Equal("global@test.com", from.Email);
                    Assert.Equal("to@test.com", to.First().Email);
                    Assert.Equal("test", message);
                });

            services.AddScoped<CustomAssertMailer>();
            var provider = services.BuildServiceProvider();

            var mailer = new CanSendMailWrapper<CustomAssertMailer>(
                RazorRendererFactory.MakeInstance(new ConfigurationBuilder().Build()),
                provider.GetRequiredService<IServiceScopeFactory>(), 
                new MailRecipient("global@test.com")
            );

            await mailer.SendAsync(
                new GenericHtmlMailable()
                    .Subject("test")
                   // .From("from@test.com") -> test should use the global from
                    .To("to@test.com")
                    .Html("test")
            );
        }

        [Fact]
        public async Task when_using_global_from_when_from_is_defined_it_uses_from_value()
        {
            var services = new ServiceCollection();
            services.AddScoped<Action<string, string, IEnumerable<MailRecipient>, MailRecipient, MailRecipient, IEnumerable<MailRecipient>, IEnumerable<MailRecipient>, IEnumerable<Attachment>, MailRecipient>>(p =>
                (message, subject, to, from, replyTo, cc, bcc, attachments, sender) =>
                { 
                    Assert.Equal("test", message);
                    Assert.Equal("from@test.com", from.Email);
                    Assert.Equal("to@test.com", to.First().Email);
                    Assert.Equal("test", message);
                });

            services.AddScoped<CustomAssertMailer>();
            var provider = services.BuildServiceProvider();

            var mailer = new CanSendMailWrapper<CustomAssertMailer>(
                RazorRendererFactory.MakeInstance(new ConfigurationBuilder().Build()),
                provider.GetRequiredService<IServiceScopeFactory>(), 
                new MailRecipient("global@test.com")
            );

            await mailer.SendAsync(
                new GenericHtmlMailable()
                    .Subject("test")
                    .From("from@test.com") // This should override the global from.
                    .To("to@test.com")
                    .Html("test")
            );
        }

        [Fact]
        public async Task when_using_render_it_renders()
        {
            var services = new ServiceCollection();
            services.AddScoped<Action<string, string, IEnumerable<MailRecipient>, MailRecipient, MailRecipient, IEnumerable<MailRecipient>, IEnumerable<MailRecipient>, IEnumerable<Attachment>, MailRecipient>>(p =>
                (message, subject, to, from, replyTo, cc, bcc, attachments, sender) =>
                { 

                });

            services.AddScoped<CustomAssertMailer>();
            var provider = services.BuildServiceProvider();

            var mailer = new CanSendMailWrapper<CustomAssertMailer>(
                RazorRendererFactory.MakeInstance(new ConfigurationBuilder().Build()),
                provider.GetRequiredService<IServiceScopeFactory>(), 
                null
            );

            var htmlMessage = await mailer.RenderAsync(
                new GenericHtmlMailable()
                    .Subject("test")
                    .From("from@test.com") 
                    .To("to@test.com")
                    .Html("<html></html>")       
            );

            Assert.Equal("<html></html>", htmlMessage);
        }
        
        [Fact]
        public async Task when_attachments_it_works()
        {
            var services = new ServiceCollection();
            services.AddScoped<Action<string, string, IEnumerable<MailRecipient>, MailRecipient, MailRecipient, IEnumerable<MailRecipient>, IEnumerable<MailRecipient>, IEnumerable<Attachment>, MailRecipient>>(p =>
                (message, subject, to, from, replyTo, cc, bcc, attachments, sender) =>
                { 
                    Assert.Equal(2, attachments.Count());
                    Assert.Equal("Attachment 2", attachments.Skip(1).Single().Name);
                });

            services.AddScoped<CustomAssertMailer>();
            var provider = services.BuildServiceProvider();

            var mailer = new CanSendMailWrapper<CustomAssertMailer>(
                RazorRendererFactory.MakeInstance(new ConfigurationBuilder().Build()),
                provider.GetRequiredService<IServiceScopeFactory>(), 
                null
            );

            await mailer.SendAsync(
                new GenericHtmlMailable()
                    .Subject("test")
                    .From("from@test.com")
                    .To("to@test.com")
                    .Html("test")
                    .Attach(new Attachment
                    {
                        Bytes = new byte[] { },
                        Name =  "Attachment 1"
                    })
                    .Attach(new Attachment
                    {
                        Bytes = new byte[] { },
                        Name =  "Attachment 2"
                    })
            );
        }

        [Fact]
        public async Task when_assigning_sender_it_is_assigned()
        {
            var services = new ServiceCollection();
            services.AddScoped<Action<string, string, IEnumerable<MailRecipient>, MailRecipient, MailRecipient, IEnumerable<MailRecipient>, IEnumerable<MailRecipient>, IEnumerable<Attachment>, MailRecipient>>(p =>
                (message, subject, to, from, replyTo, cc, bcc, attachments, sender) =>
                { 
                    Assert.Equal("sender@test.com", sender.Email);
                });

            services.AddScoped<CustomAssertMailer>();
            var provider = services.BuildServiceProvider();

            var mailer = new CanSendMailWrapper<CustomAssertMailer>(
                RazorRendererFactory.MakeInstance(new ConfigurationBuilder().Build()),
                provider.GetRequiredService<IServiceScopeFactory>(), 
                null
            );

            await mailer.SendAsync(
                new GenericHtmlMailable()
                    .Subject("test")
                    .From("from@test.com")
                    .To("to@test.com")
                    .Sender("sender@test.com")
                    .Html("test")
            );
        }
    }
}