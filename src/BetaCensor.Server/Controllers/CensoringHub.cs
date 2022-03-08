using BetaCensor.Core.Messaging;
using BetaCensor.Workers;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace BetaCensor.Server.Controllers;

public class CensoringHub : Hub<ICensorServiceClient> {
    private readonly ILogger<CensoringHub> _logger;
    private readonly IMediator _mediator;
    private readonly IAsyncBackgroundQueue<CensorImageRequest> _queue;
    private readonly IServiceScopeFactory _scopeFactory;

    // private readonly IBus _bus;

    public CensoringHub(IMediator mediator, ILogger<CensoringHub> logger, IAsyncBackgroundQueue<CensorImageRequest> requestQueue, IServiceScopeFactory scopeFactory) {
        _logger = logger;
        _mediator = mediator;
        _queue = requestQueue;
        _scopeFactory = scopeFactory;
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

    // public async Task<bool> OpenCensoringSession([FromServices]ModelLoader loader, string sessionId, IEnumerable<CensorImageRequest> requests) {
    //     var model = await loader.GetModel();
    //     var context = new CensorContext();
    //     context.Build(model, services => {
    //         services.AddHostedService<NotificationWorkerService>();
    //     })
    // }
}
