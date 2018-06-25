using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Timing;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.HostedService
{
    internal class SchedulerHost : IHostedService, IDisposable
    {
        private CancellationTokenSource _shutdown = new CancellationTokenSource();
        private static Schedule.Scheduler _scheduler;
         private SemaphoreSlim _signal = new SemaphoreSlim(0);
          private Timer _timer;

        public static Scheduler GetSchedulerInstance()
        {
            if (_scheduler == null)
                _scheduler = new Scheduler();
            return _scheduler;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._timer = new Timer((state) => this._signal.Release(),null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            Task.Run(RunScheduler);
            return Task.CompletedTask;
        }

        private async Task RunSchedulerAsync()
        {
            while (!this._shutdown.IsCancellationRequested)
            {
                await this._signal.WaitAsync(this._shutdown.Token);
                await GetSchedulerInstance().RunSchedulerAsync();                
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._shutdown.Cancel();
             this._timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this._timer?.Dispose();
            GetSchedulerInstance()?.Dispose();
        }
    }
}