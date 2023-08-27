using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Queuing.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Coravel.Queuing.HostedService;

internal sealed class QueuingHost : IHostedService, IDisposable
{
    private readonly CancellationTokenSource _shutdown = new();
    private readonly SemaphoreSlim _signal = new(0);
    private Timer? _timer;
    private readonly Queue _queue;
    private readonly IConfiguration _configuration;
    private readonly ILogger<QueuingHost> _logger;
    private const string QueueRunningMessage = "Coravel Queuing service is attempting to close but the queue is still running." +
                                                  " App closing (in background) will be prevented until dequeued tasks are completed.";

    public QueuingHost(IQueue queue, IConfiguration configuration, ILogger<QueuingHost> logger)
    {
        _configuration = configuration;
        _queue = queue as Queue ?? throw new ArgumentNullException(nameof(queue));
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        int consummationDelay = GetConsummationDelay();

        _timer = new Timer((state) => _signal.Release(), null, TimeSpan.Zero, TimeSpan.FromSeconds(consummationDelay));
        Task.Run(ConsumeQueueAsync, cancellationToken);
        return Task.CompletedTask;
    }

    private int GetConsummationDelay()
    {
        var configurationSection = _configuration.GetSection("Coravel:Queue:ConsummationDelay");
        bool couldParseDelay = int.TryParse(configurationSection.Value, out var parsedDelay);
        return couldParseDelay ? parsedDelay : 30;
    }

    private async Task ConsumeQueueAsync()
    {
        while (!_shutdown.IsCancellationRequested)
        {
            await _signal.WaitAsync(_shutdown.Token);
            await _queue.ConsumeQueueAsync();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _shutdown.Cancel();
        _timer?.Change(Timeout.Infinite, 0);

        await _queue.ConsumeQueueOnShutdown();

        // If a previous queue consummation is still running (due to some long-running queued task)
        // we don't want to shutdown while it is still running.
        if (_queue.IsRunning)
        {
            _logger.LogWarning(QueueRunningMessage);
        }

        while (_queue.IsRunning)
        {
            await Task.Delay(50, cancellationToken);
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Coravel's Queuing service is now stopped.");
        }
    }
}