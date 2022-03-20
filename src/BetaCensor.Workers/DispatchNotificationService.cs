using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MediatR;

namespace BetaCensor.Workers;

public class DispatchNotificationService<TResponse> : BackgroundService where TResponse : INotification
{
    private readonly IAsyncBackgroundQueue<TResponse> _queue;
    private readonly IMediator _mediator;
    private readonly ILogger<DispatchNotificationService<TResponse>> _logger;
    private readonly string _workerId;

    public DispatchNotificationService(IAsyncBackgroundQueue<TResponse> queue, 
        IMediator mediator, ILogger<DispatchNotificationService<TResponse>> logger)
        => (_queue, _mediator, _logger, _workerId) = (queue, mediator, logger, $"{nameof(DispatchNotificationService<TResponse>)}-{Guid.NewGuid()}");
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{_workerId} is running.");

        return ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await _queue.Dequeue(stoppingToken);
                if (workItem != null)
                {
                    _logger.LogDebug($"{_workerId} publishing notification '{workItem.ToString()}'");
                    await _mediator.Publish<TResponse>(workItem);
                }
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred publishing notification for completed work item.");
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            $"{_workerId} is stopping.");

        await base.StopAsync(stoppingToken);
    }
}