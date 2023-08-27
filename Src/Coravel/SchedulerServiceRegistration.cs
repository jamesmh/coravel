using System;
using Coravel.Events.Interfaces;
using Coravel.Scheduling.HostedService;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Scheduling.Schedule.Mutex;
using Microsoft.Extensions.DependencyInjection;

namespace Coravel;

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
        services.AddSingleton<IScheduler>(option =>
            new Scheduler(
                option.GetRequiredService<IMutex>(), 
                option.GetRequiredService<IServiceScopeFactory>(),
                option.GetService<IDispatcher>() ?? throw new ArgumentNullException(nameof(option))
            )
        );
        services.AddHostedService<SchedulerHost>();
        return services;
    }

    /// <summary>
    /// Configures the scheduler services using the service provider and a delegate to assign scheduled tasks.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <param name="assignScheduledTasks">The delegate that assigns scheduled tasks to the scheduler.</param>
    /// <returns>A scheduler configuration object.</returns>
    public static ISchedulerConfiguration UseScheduler(this IServiceProvider provider, Action<IScheduler> assignScheduledTasks)
    {
        var scheduler = provider.GetRequiredService<IScheduler>();
        assignScheduledTasks(scheduler);
        return scheduler as Scheduler ?? throw new NullReferenceException();
    }
}