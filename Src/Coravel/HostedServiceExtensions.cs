using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Coravel.Scheduling.HostedService;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Queuing.Interfaces;
using Coravel.Queuing.HostedService;
using Coravel.Queuing;
using Coravel.Cache.Interfaces;
using Coravel.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace Coravel
{
    public static class HostedServiceExtensions
    {
        /// <summary>
        /// Add Coravel's Scheduler to your app.
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

        /// <summary>
        /// Add Coravel's queueing to your app.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns></returns>
        public static IQueueConfiguration AddQueue(this IServiceCollection services)
        {
            Queue queue = new Queue();
            services.AddSingleton<IQueue>(queue);
            services.AddHostedService<QueuingHost>();

            return queue;
        }

        public static void AddCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICache>(provider =>
                new InMemoryCache(provider.GetService<IMemoryCache>())
            );
        }        
    }
}