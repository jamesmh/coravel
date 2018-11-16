using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Coravel;
using Microsoft.Extensions.Logging;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Queuing.Interfaces;
using Demo.Invocables;
using System.Threading;
using Coravel.Events.Interfaces;
using Demo.Listeners;
using Demo.Events;

namespace Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IServiceProvider services)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Coravel Scheduling
            services.AddScheduler();

            // Coravel Queuing
            services.AddQueue();               

            // Coravel Caching
            services.AddCache();

            // Coravel Mail
            services.AddMailer(this.Configuration);

            services.AddScoped<SendNightlyReportsEmailJob>();
            services.AddScoped<DoExpensiveCalculationAndStore>();

            services.AddEvents();

            services.AddTransient<WriteMessageToConsoleListener>()
                    .AddTransient<WriteStaticMessageToConsoleListener>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            IEventRegistration registration = app.ApplicationServices.ConfigureEvents();

            registration.Register<DemoEvent>()
                .Subscribe<WriteMessageToConsoleListener>()
                .Subscribe<WriteStaticMessageToConsoleListener>(); 

            app.ApplicationServices.UseScheduler(scheduler =>
            {
                // scheduler.Schedule(() => Console.WriteLine($"Every minute (ran at ${DateTime.UtcNow}) on thread {Thread.CurrentThread.ManagedThreadId}"))
                //     .EveryMinute();

                // scheduler.Schedule(() => Console.WriteLine($"Every minute#2 (ran at ${DateTime.UtcNow}) on thread {Thread.CurrentThread.ManagedThreadId}"))
                //     .EveryMinute();

                // scheduler.ScheduleAsync(async () => 
                // {
                //     await Task.Delay(10000);
                //     Console.WriteLine($"Every minute#3 (ran at ${DateTime.UtcNow}) on thread {Thread.CurrentThread.ManagedThreadId}");
                // })
                // .EveryMinute();

                // scheduler.Schedule(() => Console.WriteLine($"Every five minutes (ran at ${DateTime.UtcNow}) on thread {Thread.CurrentThread.ManagedThreadId}"))
                //     .EveryFiveMinutes();

                // scheduler.Schedule<SendNightlyReportsEmailJob>()
                //     .Cron("* * * * *")
                //     .PreventOverlapping("SendNightlyReportsEmailJob");

                //     scheduler.ScheduleAsync(async () => {
                //         var dispatcher = app.ApplicationServices.GetRequiredService<IDispatcher>();
                //         await dispatcher.Broadcast(new DemoEvent("This event happens every minute!"));
                //     }).EveryMinute();

                scheduler.OnWorker("EmailTasks");
                scheduler
                    .Schedule<SendNightlyReportsEmailJob>().Daily();
                scheduler
                    .Schedule<SendPendingNotifications>().EveryMinute();

                scheduler.OnWorker("CPUIntensiveTasks");
                scheduler
                    .Schedule<RebuildStaticCachedData>().Hourly();
            });

            app.ApplicationServices
                .ConfigureQueue()
                .LogQueuedTaskProgress(app.ApplicationServices.GetService<ILogger<IQueue>>());

            app.ApplicationServices.ConfigureQueue()
                .LogQueuedTaskProgress(app.ApplicationServices.GetService<ILogger<IQueue>>());

           
        }
    }
}
