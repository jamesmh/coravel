using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Queuing.Interfaces;
using Microsoft.Extensions.Configuration;
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
        private IConfiguration _configuration;

        public QueuingHost(IQueue queue, IConfiguration configuration)
        {
            this._configuration = configuration;
            this._queue = queue as Queue;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            int consummationDelay = GetConsummationDelay();

            this._timer = new Timer((state) => this._signal.Release(), null, TimeSpan.Zero, TimeSpan.FromSeconds(consummationDelay));
            Task.Run(ConsumeQueueAsync);
            return Task.CompletedTask;
        }

        private int GetConsummationDelay()
        {
            var configurationSection = this._configuration.GetSection("Coravel:Queue:ConsummationDelay");
            bool couldParseDelay = int.TryParse(configurationSection.Value, out var parsedDelay);
            return couldParseDelay ? parsedDelay : 30;
        }

        private async Task ConsumeQueueAsync()
        {
            while (!this._shutdown.IsCancellationRequested)
            {
                await this._signal.WaitAsync(this._shutdown.Token);
                await this._queue.ConsumeQueueAsync();
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

            // Consume the queue one last time.
            // Even if StopAsync() isn't called (uncaught app error, etc.), Dispose() is called.
            this._queue.ConsumeQueueAsync().GetAwaiter().GetResult();
        }
    }
}