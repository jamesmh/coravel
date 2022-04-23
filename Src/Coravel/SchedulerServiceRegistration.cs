using System;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.HostedService;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Scheduling.Schedule.Mutex;
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
        /// <returns></returns>
        public static IServiceCollection AddScheduler(this IServiceCollection services)
        {
            services.AddSingleton<IMutex>(new InMemoryMutex());
            services.AddSingleton<IScheduler>(p =>
                new Scheduler(
                    p.GetRequiredService<IMutex>(), 
                    p.GetRequiredService<IServiceScopeFactory>(),
                    p.GetService<IDispatcher>()
                )
            );
            services.AddHostedService<SchedulerHost>();
            return services;
        }

        public static ISchedulerConfiguration UseScheduler(this IServiceProvider provider, Action<IScheduler> assignScheduledTasks)
        {
            var scheduler = provider.GetRequiredService<IScheduler>();
            assignScheduledTasks(scheduler);
            return scheduler as Scheduler;
        }
    }
}