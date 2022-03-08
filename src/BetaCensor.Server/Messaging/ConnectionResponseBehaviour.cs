using BetaCensor.Core.Messaging;
using BetaCensor.Server.Controllers;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace BetaCensor.Server.Messaging;

public class ConnectionResponseBehaviour : IPipelineBehavior<CensorImageRequest, CensorImageResponse> {
    private readonly IHubContext<CensoringHub, ICensorServiceClient> _hub;
    private readonly ILogger<ConnectionInfo> _logger;

    public ConnectionResponseBehaviour(IHubContext<CensoringHub, ICensorServiceClient> hubContext, ILogger<ConnectionInfo> logger) {
        this._hub = hubContext;
        this._logger = logger;
    }
    public async Task<CensorImageResponse> Handle(CensorImageRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<CensorImageResponse> next) {
        var result = await next();
        if (!string.IsNullOrWhiteSpace(request.RequestId)) {
            _logger.LogInformation($"Running notification handler for {result.RequestId}");
            if (result != null) {
                await _hub.Clients.Group(request.RequestId).HandleCensoredImage(result);
            }
        }
        return result!;
    }
}
