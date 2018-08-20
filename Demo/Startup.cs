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
            services.AddScheduler();

            // Coravel Queuing
            services
                .AddQueue()
                .LogQueuedTaskProgress(Services.GetService<ILogger<IQueue>>());

            // Coravel Caching
            services.AddCache();

            // Coravel Mail
            services.AddMailer(this.Configuration);
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

            app.UseScheduler(scheduler =>
            {
                scheduler.Schedule(() => Console.WriteLine($"Every minute (ran at ${DateTime.UtcNow})"))
                .EveryMinute();

                scheduler.Schedule(() => Console.WriteLine($"Every five minutes (ran at ${DateTime.UtcNow})"))
                .EveryFiveMinutes();
            });
        }
    }
}
