using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Coravel.Scheduling.HostedService;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Queuing.Interfaces;
using Coravel.Queuing.HostedService;
using Coravel.Queuing;

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

        public static IHostedQueue AddQueue(this IServiceCollection services) {
            Queue queue = QueuingHost.GetQueueInstance();            
            services.AddHostedService<QueuingHost>();          
            services.AddSingleton<IQueue>(queue);

            return queue;
        }
    }
}