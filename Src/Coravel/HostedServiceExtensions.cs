using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Coravel.Scheduling.HostedService;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Queuing.Interfaces;

namespace Coravel
{
    public static class HostedServiceExtensions
    {
        public static IHostedScheduler AddScheduler(this IServiceCollection services, Action<IScheduler> configScheduledTasks)
        {
            Scheduler scheduler = SchedulerHost.GetSchedulerInstance();

            services.AddHostedService<SchedulerHost>();
            configScheduledTasks(scheduler);

            return scheduler; 
        }

        public static void AddQueue(this IServiceCollection services) {
            Scheduler scheduler = SchedulerHost.GetSchedulerInstance();
            IQueue queue = scheduler.UseQueue();
            services.AddSingleton<IQueue>(queue);
        }
    }
}