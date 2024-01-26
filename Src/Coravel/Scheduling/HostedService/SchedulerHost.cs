using System;
using System.Collections.Generic;
using System.Linq;
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
        private IHostApplicationLifetime _lifetime;
        private object _tickLockObj = new object();
        private EnsureContinuousSecondTicks _ensureContinuousSecondTicks;
        private readonly string ScheduledTasksRunningMessage = "Coravel's Scheduling service is attempting to close but there are tasks still running." +
                                                               " App closing (in background) will be prevented until all tasks are completed.";

        public SchedulerHost(IScheduler scheduler, ILogger<SchedulerHost> logger, IHostApplicationLifetime lifetime)
        {
            this._scheduler = scheduler as Scheduler;
            this._logger = logger;
            this._lifetime = lifetime;
            this._ensureContinuousSecondTicks = new EnsureContinuousSecondTicks(DateTime.UtcNow);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Certain features rely on the first run of the scheduler having access to all the registered
            // container services. These are only available after the full application has started. Normally, background
            // services like `IHostedService` start running before the app has fully started and therefore won't have
            // access to all the registered services right away.
            this._lifetime.ApplicationStarted.Register(InitializeAfterAppStarted);
            return Task.CompletedTask;
        }

        private void InitializeAfterAppStarted()
        {
            this._timer = new Timer(this.RunSchedulerPerSecondAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private async void RunSchedulerPerSecondAsync(object state)
        {
            if (this._schedulerEnabled)
            {
                // This will get any missed ticks that might arise from the Timer triggering a little too late. 
                // If under CPU load or if the Timer is for some reason a little slow, then it's possible to
                // miss a tick - which we want to make sure the scheduler doesn't miss and catches up.
                var now = DateTime.UtcNow;
                DateTime[] ticks = null;
                lock (_tickLockObj)
                {
                    // This class isn't thread-safe.
                    ticks = this._ensureContinuousSecondTicks.GetTicksBetweenPreviousAndNext(now).ToArray();
                    this._ensureContinuousSecondTicks.SetNextTick(now);
                }

                if (ticks.Length > 0)
                {
                    this._logger.LogInformation($"Coravel's scheduler is behind {ticks.Length} ticks and is catching-up to the current tick. Triggered at {now.ToString("o")}.");
                    foreach (var tick in ticks)
                    {
                        await this._scheduler.RunAtAsync(tick);
                    }
                }

                // If we've processed any missed ticks, we also need to explicitly run the current tick.
                await this._scheduler.RunAtAsync(now);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this._schedulerEnabled = false; // Prevents changing the timer from firing scheduled tasks.
            this._timer?.Change(Timeout.Infinite, 0);

            this._scheduler.CancelAllCancellableTasks();

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