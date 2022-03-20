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
                await _hubContext.Clients.Group(result.RequestId).HandleCensoredImage(result);
            }
            else {
                await _hubContext.Clients.Group(result.RequestId).OnCensoringError(result.RequestId, result!.Error);
            }
        }
    }
}