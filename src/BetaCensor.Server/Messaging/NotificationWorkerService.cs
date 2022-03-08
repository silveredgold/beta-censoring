using BetaCensor.Core.Messaging;
using BetaCensor.Workers;
using Microsoft.AspNetCore.SignalR;

namespace BetaCensor.Server.Messaging;

public class NotificationWorkerService : BackgroundService {
    private readonly IAsyncBackgroundQueue<CensorImageResponse> _queue;
    private readonly ILogger<NotificationWorkerService> _logger;
    private readonly IHubContext<Controllers.CensoringHub, Controllers.ICensorServiceClient> _hubContext;

    public NotificationWorkerService(IAsyncBackgroundQueue<CensorImageResponse> queue, ILogger<NotificationWorkerService> logger, IHubContext<Controllers.CensoringHub, Controllers.ICensorServiceClient> hubContext)
        => (_queue, _logger, _hubContext) = (queue, logger, hubContext);

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        _logger.LogInformation($"{nameof(NotificationWorkerService)} is running.");

        return ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            try {
                var result = await _queue.Dequeue(stoppingToken);
                if (!string.IsNullOrWhiteSpace(result?.RequestId)) {
                    _logger.LogInformation($"Running notification handler for {result.RequestId}");
                    if (result?.CensoredImage != null) {
                        await _hubContext.Clients.Group(result.RequestId).HandleCensoredImage(result);
                    }
                    else {
                        await _hubContext.Clients.Group(result!.RequestId).OnCensoringError(result!.RequestId, result!.Error);
                    }
                }
            }
            catch (OperationCanceledException) {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error occurred executing task work item.");
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken) {
        _logger.LogInformation(
            $"{nameof(NotificationWorkerService)} is stopping.");

        await base.StopAsync(stoppingToken);
    }
}
