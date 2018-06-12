using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Scheduler
{
    public static class IHostedServiceExtensions
    {
        public static void AddScheduler(this IServiceCollection services, IEnumerable<Action> scheduledTasks)
        {
            SchedulerHost.UsingScheduledTasks(scheduledTasks);
            services.AddHostedService<SchedulerHost>();
        }
    }
}