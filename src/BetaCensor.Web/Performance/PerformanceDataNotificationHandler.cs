using BetaCensor.Core.Messaging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BetaCensor.Web.Performance;

public class PerformanceDataNotificationHandler : INotificationHandler<CensorImageResponse> {
    private readonly ILogger<PerformanceDataNotificationHandler> _logger;
    private readonly IServiceScopeFactory scopeFactory;

    public PerformanceDataNotificationHandler(ILogger<PerformanceDataNotificationHandler> logger, IServiceScopeFactory scopeFactory) {
        _logger = logger;
        this.scopeFactory = scopeFactory;
    }
    public Task Handle(CensorImageResponse result, CancellationToken cancellationToken) {
        _logger.LogTrace("Running performance data notification handler!");
        if (result?.CensoringMetadata != null && result?.ImageResult?.Session != null && result.ImageResult.Results.Any()) {
            try {
                using var scope = scopeFactory.CreateScope();
                _logger.LogDebug($"Saving performance data from session ('{result.RequestId}') to cache!");
                using var db = scope.ServiceProvider.GetRequiredService<IPerformanceDataService>();
                db.AddRecord(new PerformanceRecord(result));
                // var records = db.GetCollection<PerformanceRecord>("requests");
                // records.Insert(new PerformanceRecord(result));
            }
            catch (Exception e) {
                _logger.LogWarning(e, $"Error encountered while saving performance data for ${result.RequestId}.");
            }
        }
        return Task.CompletedTask;
    }
}
