using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Coravel.Scheduling.HostedService;
using Coravel.Scheduling.Schedule;

namespace Coravel.Scheduling
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