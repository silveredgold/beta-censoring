using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BetaCensor.Workers;

public class WorkerControllerService<TWorker> : BackgroundService where TWorker : IHostedService
{
    private readonly ILogger<WorkerControllerService<TWorker>> _logger;
    private readonly IServiceProvider _provider;
    private readonly int _numWorkers;
    private readonly IServiceScopeFactory _scopeFactory;

    public WorkerControllerService(ILogger<WorkerControllerService<TWorker>> logger, IServiceProvider provider, IServiceScopeFactory scopeFactory, int numberOfWorkers)
    {
        _logger = logger;
        _provider = provider;
        _numWorkers = numberOfWorkers;
        _scopeFactory = scopeFactory;
    }

    public WorkerControllerService(ILogger<WorkerControllerService<TWorker>> logger, IServiceProvider provider, IServiceScopeFactory scopeFactory) : this(logger, provider, scopeFactory, 2)
    {
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workers = new List<Task>();
                for (int i = 0; i < _numWorkers; i++)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        workers.Add(scope.ServiceProvider.GetRequiredService<TWorker>().StartAsync(stoppingToken));
                    }
                    // workers.Add(_provider.GetRequiredService<TWorker>().StartAsync(stoppingToken));
                }
                await Task.WhenAll(workers.ToArray());
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing task work item.");
            }

        }
    }
}
