using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Scheduler
{
    public class ScheduleHostTimer : IHostedService, IDisposable
    {
        private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        private IEnumerable<Action> _tasks;
        private Timer _timer;

        public ScheduleHostTimer(IEnumerable<Action> tasks)
        {
            this._tasks = tasks;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._timer = new Timer(this.RunScheduler, null, TimeSpan.Zero, OneMinute);
            return Task.CompletedTask;
        }

        private void RunScheduler(object state)
        {
            this.StopTimer();

            foreach (var task in this._tasks)
            {
                task();
            }

            this.StartTimer();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._tasks = null;
            this._timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void StopTimer()
        {
            this._timer?.Change(Timeout.Infinite, 0);
        }

        private void StartTimer()
        {
            this._timer.Change(TimeSpan.Zero, OneMinute);
        }

        public void Dispose()
        {
            this._timer?.Dispose();
        }
    }
}