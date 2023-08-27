using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.Logging;

namespace Coravel.Scheduling.HostedService;

internal sealed class SchedulerHost : IHostedService, IDisposable
{
    private readonly Scheduler? _scheduler;
    private Timer? _timer;
    private bool _schedulerEnabled = true;
    private readonly ILogger<SchedulerHost> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private const string ScheduledTasksRunningMessage = "Coravel's Scheduling service is attempting to close but there are tasks still running." +
                                                           " App closing (in background) will be prevented until all tasks are completed.";

    public SchedulerHost(IScheduler scheduler, ILogger<SchedulerHost> logger, IHostApplicationLifetime lifetime)
    {
        _scheduler = scheduler as Scheduler;
        _logger = logger;
        _lifetime = lifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Certain features rely on the first run of the scheduler having access to all the registered
        // container services. These are only available after the full application has started. Normally, background
        // services like `IHostedService` start running before the app has fully started and therefore won't have
        // access to all the registered services right away.
        _lifetime.ApplicationStarted.Register(InitializeAfterAppStarted);
        return Task.CompletedTask;
    }

    private void InitializeAfterAppStarted()
    {
        _timer = new Timer(RunSchedulerPerSecondAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private async void RunSchedulerPerSecondAsync(object state)
    {
        if (_schedulerEnabled && _scheduler != null)
        {
            await _scheduler.RunSchedulerAsync();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _schedulerEnabled = false; // Prevents changing the timer from firing scheduled tasks.
        _timer?.Change(Timeout.Infinite, 0);

        _scheduler?.CancelAllCancellableTasks();

        // If a previous scheduler execution is still running (due to some long-running scheduled task[s])
        // we don't want to shutdown while they are still running.
        if (_scheduler != null && _scheduler.IsRunning)
        {
            _logger.LogWarning(ScheduledTasksRunningMessage);
        }

        while (_scheduler != null && _scheduler.IsRunning)
        {
            await Task.Delay(50, cancellationToken);
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Coravel's Scheduling service is now stopped.");
        }
    }
}