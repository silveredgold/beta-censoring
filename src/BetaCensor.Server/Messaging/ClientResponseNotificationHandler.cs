using BetaCensor.Core.Messaging;
using BetaCensor.Server.Controllers;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace BetaCensor.Server.Messaging;

public class ClientResponseNotificationHandler : INotificationHandler<CensorImageResponse> {
    private readonly IHubContext<CensoringHub, ICensorServiceClient> _hubContext;
    private readonly ILogger<ClientResponseNotificationHandler> _logger;

    public ClientResponseNotificationHandler(IHubContext<CensoringHub, ICensorServiceClient> hubContext, ILogger<ClientResponseNotificationHandler> logger) {
        _hubContext = hubContext;
        _logger = logger;
    }
    public async Task Handle(CensorImageResponse result, CancellationToken cancellationToken) {
        if (result?.RequestId != null && !string.IsNullOrWhiteSpace(result.RequestId)) {
            if (result.CensoredImage != null) {
                _logger.LogDebug($"Processing response for {result.RequestId}: ({result.CensoredImage?.ImageContents?.Length ?? -1}b/{result.CensoredImage?.MimeType})");
                try
                {
                    await _hubContext.Clients.Group(result.RequestId).HandleCensoredImage(result);
                } catch (Exception e)
                {
                    _logger.LogError(e, "Error when writing client response");
                }
            }
            else {
                await _hubContext.Clients.Group(result.RequestId).OnCensoringError(result.RequestId, result!.Error);
            }
        }
    }
}