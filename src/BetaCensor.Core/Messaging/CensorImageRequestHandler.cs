using CensorCore;
using CensorCore.Censoring;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BetaCensor.Core.Messaging;

public class CensorImageRequestHandler : IRequestHandler<CensorImageRequest, CensorImageResponse>
{
    private readonly AIService _ai;
    private readonly ICensoringProvider _censor;
    private readonly ILogger<CensorImageRequestHandler> _logger;

    public CensorImageRequestHandler(AIService aiService, ICensoringProvider censoringProvider, ILogger<CensorImageRequestHandler> logger)
    {
        _logger = logger;
        // _logger.LogInformation($".ctor: {DateTime.UtcNow.ToString("o")}");
        this._ai = aiService;
        this._censor = censoringProvider;

    }
    public async Task<CensorImageResponse> Handle(CensorImageRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Processing censoring request: {request.RequestId}");
        var imageUrl = request.ImageDataUrl ?? request.ImageUrl;
        if (!string.IsNullOrWhiteSpace(imageUrl)) {
            var timer = new System.Diagnostics.Stopwatch();
            try {
                var result = await this._ai.RunModel(imageUrl);
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
            } catch (Exception e) {
                return MessageResponse.GetError<CensorImageResponse>(request.RequestId, $"Error encountered while censoring this image: {e.ToString()} ({e.Message})");
            }
        } else {
            return MessageResponse.GetError<CensorImageResponse>(request.RequestId, "Could not determine image URL from request!");
        }
    }
}
