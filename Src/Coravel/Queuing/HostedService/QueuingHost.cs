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
        private ILogger<QueuingHost> _logger;
        private readonly string QueueRunningMessage = "Coravel Queuing service is attempting to close but the queue is still running." +
                                                      " App closing (in background) will be prevented until dequeued tasks are completed.";

        public QueuingHost(IQueue queue, IConfiguration configuration, ILogger<QueuingHost> logger)
        {
            this._configuration = configuration;
            this._queue = queue as Queue;
            this._logger = logger;
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

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            this._shutdown.Cancel();
            this._timer?.Change(Timeout.Infinite, 0);            

            // Consume the queue one last time.
            await this._queue.ConsumeQueueAsync();

            // If a previous queue consummation is still running (due to some long-running queued task)
            // we don't want to shutdown while it is still running.
            if(this._queue.IsRunning){
                this._logger.LogWarning(QueueRunningMessage);
            }

            while(this._queue.IsRunning)
            {
                await Task.Delay(50);
            }
        }

        public void Dispose()
        {
            this._timer?.Dispose();
        }
    }
}