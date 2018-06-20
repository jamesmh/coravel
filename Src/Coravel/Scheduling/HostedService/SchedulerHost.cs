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
        private static Schedule.Scheduler _scheduler;
        private OneMinuteTimer _timer;

        public static Scheduler GetSchedulerInstance()
        {
            if(_scheduler == null)
                _scheduler = new Scheduler();
            return _scheduler;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._timer = new OneMinuteTimer(this.InvokeScheduledTasks);
            return Task.CompletedTask;
        }

        private void InvokeScheduledTasks()
        {  
            GetSchedulerInstance().RunScheduledTasks(DateTime.UtcNow);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._timer?.Stop();
            return Task.CompletedTask;
        } 

        public void Dispose()
        {
            this._timer?.Dispose();
        }
    }
}