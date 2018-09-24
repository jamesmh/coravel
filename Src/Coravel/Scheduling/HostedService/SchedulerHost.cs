using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.Logging;

namespace Coravel.Scheduling.HostedService
{
    internal class SchedulerHost : IHostedService, IDisposable
    {
        private Scheduler _scheduler;
        private Timer _timer;
        private bool _schedulerEnabled = true;
        private ILogger<SchedulerHost> _logger;
        private readonly string ScheduledTasksRunningMessage = "Coravel's Scheduling service is attempting to close but there are tasks still running." +
                                                               " App closing (in background) will be prevented until all tasks are completed.";

        public SchedulerHost(IScheduler scheduler, ILogger<SchedulerHost> logger)
        {
            this._scheduler = scheduler as Scheduler;
            this._logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._timer = new Timer(this.RunSchedulerAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            return Task.CompletedTask;
        }

        private async void RunSchedulerAsync(object state)
        {
            if (this._schedulerEnabled)
            {
                await this._scheduler.RunSchedulerAsync();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this._schedulerEnabled = false; // Prevents changing the timer from firing scheduled tasks.
            this._timer?.Change(Timeout.Infinite, 0);

            // If a previous scheduler execution is still running (due to some long-running scheduled task[s])
            // we don't want to shutdown while they are still running.
            if (this._scheduler.IsRunning)
            {
                this._logger.LogWarning(ScheduledTasksRunningMessage);
            }

            while (this._scheduler.IsRunning)
            {
                await Task.Delay(50);
            }
        }

        public void Dispose()
        {
            this._timer?.Dispose();
            this._logger.LogInformation("Coravel's Scheduling service is now stopped.");
        }
    }
}