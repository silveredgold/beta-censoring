using BetaCensor.Core.Messaging;
using CensorCore;
using CensorCore.Censoring;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BetaCensor.Workers
{
    public class CensoringWorkerService : BackgroundService
    {
        private readonly IAsyncBackgroundQueue<CensorImageRequest> _queue;
        private readonly IAsyncBackgroundQueue<CensorImageResponse> _output;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CensoringWorkerService> _logger;
        private readonly AIService _ai;
        private readonly ICensoringProvider _censor;
        private readonly string _workerId;

        public CensoringWorkerService(IAsyncBackgroundQueue<CensorImageRequest> queue, IAsyncBackgroundQueue<CensorImageResponse> outputQueue, IServiceScopeFactory scopeFactory, ILogger<CensoringWorkerService> logger,
        AIService aiService, ICensoringProvider censoringProvider)
            => (_queue, _output, _scopeFactory, _logger, _ai, _censor, _workerId) = (queue, outputQueue, scopeFactory, logger, aiService, censoringProvider, $"{nameof(CensoringWorkerService)}-{Guid.NewGuid()}");
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
                if (workItem != null) {
                    _logger.LogInformation($"{_workerId} processing request '{workItem.RequestId}'");
                    var response = await CensorImage(workItem);
                    if (response != null) {
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

    public async Task<CensorImageResponse> CensorImage(CensorImageRequest request) {
        _logger.LogInformation($"Processing censoring request: {request.RequestId}");
        var imageUrl = request.ImageDataUrl ?? request.ImageUrl;
        if (!string.IsNullOrWhiteSpace(imageUrl)) {
            var timer = new System.Diagnostics.Stopwatch();
            using var scope = _scopeFactory.CreateScope();
            var options = scope.ServiceProvider.GetService<MatchOptions>();
            var result = await this._ai.RunModel(imageUrl, options);
            if (result != null) {
                timer.Start();
                IResultParser? parser = null;
                if (request.CensorOptions.Any()) {
                    parser = new StaticResultsParser(request.CensorOptions);
                }
                var censored = await this._censor.CensorImage(result, parser);
                timer.Stop();
                _logger.LogInformation($"Censoring completed in {timer.Elapsed.TotalSeconds}s ({request.RequestId}:{censored.MimeType})");
                return new CensorImageResponse {
                    RequestId = request.RequestId,
                    CensoredImage = censored,
                    ImageResult = result
                };
            } else {
                return MessageResponse.GetError<CensorImageResponse>(request.RequestId, "AI model failed to process requested image!");
            }
        } else {
            return MessageResponse.GetError<CensorImageResponse>(request.RequestId, "Could not determine image URL from request!");
        }
    }
    }
}