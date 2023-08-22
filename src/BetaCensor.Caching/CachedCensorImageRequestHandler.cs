using System.Text.Json;
using System.Text.Json.Serialization;
using BetaCensor.Core.Messaging;
using CensorCore;
using CensorCore.Censoring;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZiggyCreatures.Caching.Fusion;


namespace BetaCensor.Caching;

public class CachedCensorImageRequestHandler : IRequestHandler<CensorImageRequest, CensorImageResponse>
{
    private readonly AIService _ai;
    private readonly ICensoringProvider _censor;
    // private readonly MatchOptions? _matchOptions;
    private readonly ILogger<CachedCensorImageRequestHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IFusionCacheProvider _cacheProvider;
    private readonly CachingOptions _options;

    public CachedCensorImageRequestHandler(AIService aiService, ICensoringProvider censoringProvider, ILogger<CachedCensorImageRequestHandler> logger, IServiceScopeFactory scopeFactory, IFusionCacheProvider cacheProvider, CachingOptions options)
    => (_logger, _ai, _censor, _scopeFactory, _cacheProvider, _options) = (logger, aiService, censoringProvider, scopeFactory, cacheProvider, options);

    public async Task<CensorImageResponse> Handle(CensorImageRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Processing censoring request with caching available: {request.RequestId}");
        // var imageUrl = request.ImageDataUrl ?? request.ImageUrl;
        if (!string.IsNullOrWhiteSpace(request.ImageDataUrl) || !string.IsNullOrWhiteSpace(request.ImageUrl)) {
            var timer = new System.Diagnostics.Stopwatch();
            try {
                using var scope = _scopeFactory.CreateScope();
                var matchOptions = scope.ServiceProvider.GetService<MatchOptions>();
                var resultsCache = _cacheProvider.GetCacheOrNull(CensoringCaches.Matches);
                ImageResult? result = null;
                var requestKey = request.GetKey();
                if (resultsCache != null && _options.EnableMatchCaching) {
                    
                    result = await resultsCache.GetOrSetAsync<ImageResult>(requestKey, async _ => {
                        _logger.LogDebug($"Missed cache for {new string(requestKey.Take(32).ToArray())}, running model!");
                        var output = await RunModel(request, matchOptions);
                        return output;
                    });
                } else {
                    result = await RunModel(request, matchOptions);
                }
                // if (resultsCache != null && resultsCache.TryGet<ImageResult>(request.GetKey()) is var cacheResult && cacheResult.HasValue) {
                //     result = cacheResult.Value;
                // } else {
                //     result = await RunModel(request, matchOptions);
                //     await resultsCache?.GetOrSetAsync
                // }
                if (result != null) {
                    timer.Start();
                    IResultParser? parser = null;
                    if (request.CensorOptions.Any()) {
                        parser = new StaticResultsParser(request.CensorOptions);
                    }
                    CensoredImage? censored = null;
                    if (_options.EnableCensoringCaching) {
                        var key = GetCensoringCacheKey(result, request, requestKey);
                        var censorCache = _cacheProvider.GetCacheOrNull(CensoringCaches.Censoring);
                        if (censorCache != null && key != null) {
                            censored = await censorCache.GetOrSetAsync<CensoredImage>(key, async (ctx, _) => {
                                _logger.LogDebug($"Missed cache for {new string(key.Take(12).ToArray())}, running censoring!");
                                var output = await _censor.CensorImage(result, parser);
                                float mb = (output.ImageContents.Length / 1024f) / 1024f;
                                ctx.Options.Size = Convert.ToInt64(mb*1.1F);
                                return output;
                            });
                        } else {
                            censored = await this._censor.CensorImage(result, parser);
                        }
                    } else {
                        censored = await this._censor.CensorImage(result, parser);
                    }
                    timer.Stop();
                    _logger.LogInformation($"Censoring completed in {timer.Elapsed.TotalSeconds}s ({request.RequestId}:{censored.MimeType})");
                    return new CensorImageResponse {
                        RequestId = request.RequestId,
                        CensoredImage = censored,
                        ImageResult = result,
                        CensoringMetadata = new CensoringSession(timer.Elapsed)
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

    private async Task<ImageResult?> RunModel(CensorImageRequest request, MatchOptions options) {
        ImageResult? result = null;
        if (string.IsNullOrWhiteSpace(request.ImageDataUrl)) {
            result = await this._ai.RunModel(request.ImageUrl!, options);
        } else if (string.IsNullOrWhiteSpace(request.ImageUrl)) {
            result = await this._ai.RunModel(request.ImageDataUrl, options);
        } else {
            //we have both, so we can try them both
            try {
                result = await this._ai.RunModel(request.ImageDataUrl!, options);
            } catch {
                result = await this._ai.RunModel(request.ImageUrl!, options);
            }
        }
        return result;
    }

    private string? GetCensoringCacheKey(ImageResult result, CensorImageRequest request, string requestKey) {
        var ser = JsonSerializer.Serialize(result);
        if (!string.IsNullOrWhiteSpace(ser)) {
            var baseKey = $"{ser.GetHash()}:{requestKey.Take(64).GetHash()}";
            if (request.CensorOptions.Any()) {
                return $"{baseKey}:{JsonSerializer.Serialize(request.CensorOptions).GetHash()}";
            } else {
                return baseKey;
            }
        }
        return null;
    }
}
