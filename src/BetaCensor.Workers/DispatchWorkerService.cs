using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MediatR;

namespace BetaCensor.Workers;

public class DispatchWorkerService<TRequest, TResponse> : BackgroundService where TRequest : IRequest<TResponse>
{
    private readonly IAsyncBackgroundQueue<TRequest> _queue;
    private readonly IAsyncBackgroundQueue<TResponse> _output;
    private readonly IMediator _mediator;
    private readonly ILogger<DispatchWorkerService<TRequest, TResponse>> _logger;
    private readonly string _workerId;

    public DispatchWorkerService(IAsyncBackgroundQueue<TRequest> queue, IAsyncBackgroundQueue<TResponse> outputQueue,
        IMediator mediator, ILogger<DispatchWorkerService<TRequest, TResponse>> logger)
        => (_queue, _output, _mediator, _logger, _workerId) = (queue, outputQueue, mediator, logger, $"{nameof(DispatchWorkerService<TRequest, TResponse>)}-{Guid.NewGuid()}");
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
                    _logger.LogDebug($"{_workerId} processing request '{workItem.ToString()}'");
                    var response = await _mediator.Send<TResponse>(workItem);
                    if (response != null)
                    {
                        await _output.Enqueue(response);
                    }
                }
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

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            $"{_workerId} is stopping.");

        await base.StopAsync(stoppingToken);
    }
}
