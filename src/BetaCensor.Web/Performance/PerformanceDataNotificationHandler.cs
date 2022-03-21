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
        _logger.LogInformation("Running performance data notification handler!");
        using var scope = scopeFactory.CreateScope();
        if (result?.CensoringMetadata != null && result?.ImageResult?.Session != null) {
            try {
                _logger.LogInformation($"Saving performance data from session ('{result.RequestId}') to cache!");
                using var db = scope.ServiceProvider.GetRequiredService<IPerformanceDataService>();
                db.AddRecord(new PerformanceRecord(result));
                // var records = db.GetCollection<PerformanceRecord>("requests");
                // records.Insert(new PerformanceRecord(result));
            }
            catch {
                _logger.LogWarning($"Error encountered while saving performance data for ${result.RequestId}.");
            }
        }
        return Task.CompletedTask;
    }
}
