using System;
using Coravel.Scheduling.HostedService;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
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
        public static ISchedulerConfiguration AddScheduler(this IServiceCollection services, Action<IScheduler> assignScheduledTasks)
        {
            Scheduler scheduler = new Scheduler();
            services.AddSingleton<IScheduler>(scheduler);
            services.AddHostedService<SchedulerHost>();
            assignScheduledTasks(scheduler);

            return scheduler;
        }
    }
}