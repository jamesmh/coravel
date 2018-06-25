using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Queuing.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Coravel.Queuing.HostedService
{
    internal class QueuingHost : IHostedService, IDisposable
    {
        private CancellationTokenSource _shutdown = new CancellationTokenSource();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);
        private Timer _timer;
        private static Queue _queue;

        public static Queue GetQueueInstance()
        {
            if (_queue == null)
                _queue = new Queue();
            return _queue;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {            
            this._timer = new Timer((state) => this._signal.Release(),null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            Task.Run(ConsumeQueueAsync);
            return Task.CompletedTask;
        }

        private async Task ConsumeQueueAsync()
        {
            while (!this._shutdown.IsCancellationRequested)
            {
                await this._signal.WaitAsync(this._shutdown.Token);
                await GetQueueInstance().ConsumeQueueAsync();                         
            }
        }    

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Signal to background thread that we are done :)
            this._shutdown.Cancel();

            this._timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this._timer?.Dispose();

            // Run the scheduler one last time.
            // Even if StopAsync() isn't called (uncaught app error, etc.), Dispose() is called.
            GetQueueInstance().ConsumeQueueAsync().GetAwaiter().GetResult();

            Console.WriteLine("disposed");
        }
    }
}