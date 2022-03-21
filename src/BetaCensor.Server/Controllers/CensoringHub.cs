using BetaCensor.Core.Messaging;
using BetaCensor.Workers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BetaCensor.Server.Controllers;

public class CensoringHub : Hub<ICensorServiceClient> {
    private readonly ILogger<CensoringHub> _logger;
    private readonly IMediator _mediator;
    private readonly IAsyncBackgroundQueue<CensorImageRequest> _queue;
    private readonly QueueValidator<CensorImageRequest>? _validator;

    public CensoringHub(IMediator mediator, ILogger<CensoringHub> logger, IAsyncBackgroundQueue<CensorImageRequest> requestQueue, QueueValidator<CensorImageRequest>? validator = null) {
        _logger = logger;
        _mediator = mediator;
        _queue = requestQueue;
        _validator = validator;
    }

    public async Task<bool> CensorImage(CensorImageRequest request) {
        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();
        request.RequestId ??= Guid.NewGuid().ToString();
        _logger.LogInformation($"Got request for {request.RequestId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, request.RequestId);
        _logger.LogInformation($"Group registered for {request.RequestId}");
        try {
            var requestQueue = _queue;
            _ = requestQueue.Enqueue(request);
        }
        catch (Exception e) {
            _ = Clients.Group(request.RequestId).OnRequestUpdate(request.RequestId, e.ToString());
        }
        timer.Stop();
        _logger.LogInformation($"Action timer: {timer.Elapsed.TotalSeconds}");
        _logger.LogInformation(DateTime.UtcNow.ToString("o"));
        return true;
    }

    public async Task<CensorImageResponse> CensorImageSync(CensorImageRequest request) {
        request.RequestId ??= Guid.NewGuid().ToString();
        await Groups.AddToGroupAsync(Context.ConnectionId, request.RequestId);
        var result = await _mediator.Send(request);
        return result;
    }

    public Task CancelRequests(CancelRequest request) {
        try {
            var requestIds = request.Requests ?? new List<string>();
            if (requestIds.Any()) {
                _logger.LogTrace("Cancelling requests: " + string.Join(',', requestIds));
            }
            if (_validator != null) {
                _validator.CancelRequests(requestIds);
            }
        } catch (Exception e) {
            _logger.LogWarning(e, "Error processing cancel request!");
            //ignored
        }
        return Task.CompletedTask;
    }
}
