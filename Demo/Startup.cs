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

namespace Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IServiceProvider services)
        {
            Configuration = configuration;
            Services = services;
        }

        public IConfiguration Configuration { get; }
        public IServiceProvider Services { get; }

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
            services.AddScheduler(scheduler =>
            {
                // Run this every five minutes only on Fri and Sat.
                scheduler.Schedule(() => Console.WriteLine("Every minute"))
                .EveryMinute();

                scheduler.ScheduleAsync(async () =>
                {
                    await Task.Delay(5000);
                    Console.WriteLine("async task");
                })
                .EveryMinute();


                // Run this task every minute.
                scheduler.Schedule(() => Console.WriteLine("Saturday at xx:44"))
                .HourlyAt(44)
                .Saturday();

                scheduler.Schedule(() => Console.WriteLine("During the week at xx:05"))
                .HourlyAt(05)
                .Weekday();
            })
            .LogScheduledTaskProgress(Services.GetService<ILogger<IScheduler>>());

            // Coravel Queuing
            services
                .AddQueue()
                .LogQueuedTaskProgress(Services.GetService<ILogger<IQueue>>());
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
        }
    }
}
