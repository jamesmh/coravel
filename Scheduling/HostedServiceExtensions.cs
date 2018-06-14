using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Scheduling.HostedService;
using Scheduling.Schedule;

namespace Scheduling
{
    public static class HostedServiceExtensions
    {
        public static void AddScheduler(this IServiceCollection services, Action<Scheduler> configScheduledTasks)
        {
            services.AddHostedService<SchedulerHost>();
            configScheduledTasks(SchedulerHost.GetSchedulerInstance());
        }
    }
}