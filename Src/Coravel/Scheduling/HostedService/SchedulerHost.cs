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

        public SchedulerHost(IScheduler scheduler)
        {
            this._scheduler = scheduler as Scheduler;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._timer = new Timer(this.RunSchedulerAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            return Task.CompletedTask;
        }

        private async void RunSchedulerAsync(object state)
        {
            if(this._schedulerEnabled) {
                await this._scheduler.RunSchedulerAsync();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this._schedulerEnabled = false; // Prevents changing the timer from firing scheduled tasks.
            this._timer?.Change(Timeout.Infinite, 0);

            await WaitUntilSchedulerCompletedRunning();
        }

        public void Dispose()
        {
            this._timer?.Dispose();
        }

        private async Task WaitUntilSchedulerCompletedRunning()
        {
            while (this._scheduler.IsStillRunning())
            {
                await Task.Delay(100);
            }
        }
    }
}