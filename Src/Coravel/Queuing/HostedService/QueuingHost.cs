using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Queuing.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Coravel.Queuing.HostedService
{
    internal class QueuingHost : IHostedService, IDisposable
    {
        private CancellationTokenSource _shutdown = new CancellationTokenSource();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);
        private Timer _timer;
        private Queue _queue;
        private ILogger<QueuingHost> _logger;

        public QueuingHost(ILogger<QueuingHost> logger, IQueue queue)
        {
            this._logger = logger;
            this._queue = queue as Queue;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this._logger?.LogInformation("Queue service has started.");
            this._timer = new Timer((state) => this._signal.Release(), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            Task.Run(ConsumeQueueAsync);
            return Task.CompletedTask;
        }

        private async Task ConsumeQueueAsync()
        {
            while (!this._shutdown.IsCancellationRequested)
            {
                await this._signal.WaitAsync(this._shutdown.Token);

                this._logger?.LogInformation("Queued items are being executed.");
                await this._queue.ConsumeQueueAsync();
                this._logger?.LogInformation("Queue iteration done.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this._logger?.LogInformation("Queue service is has received request to shutdown.");

            this._shutdown.Cancel();

            this._timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this._logger?.LogInformation("Queue service is executing one last time before shutdown.");

            this._timer?.Dispose();

            // Run the scheduler one last time.
            // Even if StopAsync() isn't called (uncaught app error, etc.), Dispose() is called.
            this._queue.ConsumeQueueAsync().GetAwaiter().GetResult();

            this._logger?.LogInformation("Queue successfully disposed.");
        }
    }
}