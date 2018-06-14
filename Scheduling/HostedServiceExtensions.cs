using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Scheduling.HostedService;
using Scheduling.Schedule;

namespace Scheduling
{
    public static class HostedServiceExtensions
    {
        public static Scheduler AddScheduler(this IServiceCollection services)
        {
            services.AddHostedService<SchedulerHost>();
            return SchedulerHost.GetSchedulerInstance();
        }
    }
}