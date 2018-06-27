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
        private CancellationTokenSource _shutdown = new CancellationTokenSource();
        private Scheduler _scheduler;
        private SemaphoreSlim _signal = new SemaphoreSlim(0);
        private Timer _timer;
        private ILogger<SchedulerHost> _logger;

        public SchedulerHost(ILogger<SchedulerHost> logger, IScheduler scheduler)
        {
            this._logger = logger;
            this._scheduler = scheduler as Scheduler;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation("Schedule service starting...");

            this._timer = new Timer((state) => this._signal.Release(), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            Task.Run(RunSchedulerAsync);
            return Task.CompletedTask;
        }

        private async Task RunSchedulerAsync()
        {
            while (!this._shutdown.IsCancellationRequested)
            {
                await this._signal.WaitAsync(this._shutdown.Token);

                this._logger.LogInformation("Scheduler checking for tasks to execute...");
                await this._scheduler.RunSchedulerAsync();
                this._logger.LogInformation("Scheduler iteration completed.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._logger.LogInformation("Schedule service received shutdown request.");

            // Signal to background thread that we are done :)
            this._shutdown.Cancel();

            this._timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this._logger.LogInformation("Schedule service running one last time being shutdown.");

            this._timer?.Dispose();

            // Run the scheduler one last time.
            // Even if StopAsync() isn't called (uncaught app error, etc.), Dispose() is called.
            this._scheduler?.Dispose();

            this._logger.LogInformation("Schedule service was gracefully disposed.");
        }
    }
}