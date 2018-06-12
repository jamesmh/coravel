using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Scheduler
{
    internal class SchedulerHost : IHostedService, IDisposable
    {
        private static IEnumerable<Action> _tasks;
        private OneMinuteTimer _timer;

        public static void UsingScheduledTasks(IEnumerable<Action> tasks) {
            _tasks = tasks;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._timer = new OneMinuteTimer(this.InvokeScheduledTasks);
            return Task.CompletedTask;
        }

        private void InvokeScheduledTasks()
        {  
            foreach(var task in _tasks)
                task();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._timer?.Stop();
            return Task.CompletedTask;
        } 

        public void Dispose()
        {
            _tasks = null;
            this._timer?.Dispose();
        }
    }
}