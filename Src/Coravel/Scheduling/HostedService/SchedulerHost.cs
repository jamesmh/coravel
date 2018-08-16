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
            await this._scheduler.RunSchedulerAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this._timer?.Dispose();

            // Run the scheduler one last time.
            // Even if StopAsync() isn't called (uncaught app error, etc.), Dispose() is called.
            this._scheduler?.Dispose();
        }
    }
}