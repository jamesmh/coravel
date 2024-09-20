using System;
using System.Threading;
using Coravel;
using Coravel.Events.Interfaces;
using Coravel.Queuing.Interfaces;
using Coravel.Cache.PostgreSQL;
using Coravel.Cache.SQLServer;
using Demo.Events;
using Demo.Invocables;
using Demo.Listeners;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IServiceProvider services) => this.Configuration = configuration;

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

            services.AddMvc(options => { options.EnableEndpointRouting = false; }).AddRazorRuntimeCompilation();
            services.AddControllersWithViews();

            // Coravel Scheduling
            services.AddScheduler();

            // Coravel Queuing
            services.AddQueue();

            // Coravel Caching
            //services.AddSQLServerCache(this.Configuration.GetConnectionString("DefaultConnection"));
           // services.AddPostgreSQLCache(this.Configuration.GetConnectionString("PostGreSQL"));

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
               // app.UseHsts();
            }

           // app.UseHttpsRedirection();
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
                scheduler.OnWorker("CPUIntensiveTasks");
                scheduler
                    .Schedule<RebuildStaticCachedData>().Hourly();

                scheduler.OnWorker("TestingSeconds");
                scheduler.Schedule(
                    () => Console.WriteLine($"Runs every second. Ran at: {DateTime.UtcNow}")
                ).EverySecond();
                scheduler.Schedule(() => Console.WriteLine($"Runs every thirty seconds. Ran at: {DateTime.UtcNow}")).EveryThirtySeconds().Zoned(TimeZoneInfo.Local);
                scheduler.Schedule(() => Console.WriteLine($"Runs every ten seconds. Ran at: {DateTime.UtcNow}")).EveryTenSeconds();
                scheduler.Schedule(() => Console.WriteLine($"Runs every fifteen seconds. Ran at: {DateTime.UtcNow}")).EveryFifteenSeconds();
                scheduler.Schedule(() => Console.WriteLine($"Runs every thirty seconds. Ran at: {DateTime.UtcNow}")).EveryThirtySeconds();
                scheduler.Schedule(() => Console.WriteLine($"Runs every minute Ran at: {DateTime.UtcNow}")).EveryMinute();
                scheduler.Schedule(() => Console.WriteLine($"Runs every 2nd minute Ran at: {DateTime.UtcNow}")).Cron("*/2 * * * *");


                scheduler.Schedule(() => Thread.Sleep(5000)).EverySecond();
            });

            app.ApplicationServices
                .ConfigureQueue()
                .LogQueuedTaskProgress(app.ApplicationServices.GetService<ILogger<IQueue>>());

            app.ApplicationServices.ConfigureQueue()
                .LogQueuedTaskProgress(app.ApplicationServices.GetService<ILogger<IQueue>>());


        }
    }
}
