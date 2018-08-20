using System;
using Coravel.Scheduling.HostedService;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Scheduling.Schedule.Mutex;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel
{
    /// <summary>
    /// IServiceCollection extensions for registering Coravel's Scheduler.
    /// </summary>
    public static class SchedulerServiceRegistration
    {
        /// <summary>
        /// Register Coravel's Scheduler.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assignScheduledTasks">Action that assigns all your scheduled tasks</param>
        /// <returns></returns>
        public static void AddScheduler(this IServiceCollection services)
        {
            services.AddSingleton<IMutex>(new InMemoryMutex());
            services.AddSingleton<IScheduler>(p =>
                new Scheduler(p.GetRequiredService<IMutex>(), p.GetRequiredService<IServiceScopeFactory>()));
            services.AddHostedService<SchedulerHost>();
        }

        public static IScheduler UseScheduler(this IApplicationBuilder app, Action<IScheduler> assignScheduledTasks)
        {
            var scheduler = app.ApplicationServices.GetRequiredService<IScheduler>();
            assignScheduledTasks(scheduler);
            return scheduler;
        }
    }
}