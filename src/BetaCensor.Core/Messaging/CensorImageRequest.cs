using CensorCore;
using CensorCore.Censoring;
using MediatR;
namespace BetaCensor.Core.Messaging;

public class CensorImageRequest : IRequest<CensorImageResponse>
{
    public string? RequestId {get;set;}
    public string? ImageUrl {get;set;}
    public string? ImageDataUrl {get;set;}
    public Dictionary<string, ImageCensorOptions> CensorOptions {get;set;} = new Dictionary<string, ImageCensorOptions>();
}

public class CensorImageResponse : MessageResponse {

    public ImageResult? ImageResult {get;set;}
    public CensoredImage? CensoredImage {get;set;}
}

public abstract class MessageResponse {
    public string? Error {get;set;}
    public string? RequestId {get;set;}

    public static TResponse GetError<TResponse>(string? requestId, string message) where TResponse : MessageResponse, new() {
        return new TResponse() {
            RequestId = requestId,
            Error = message
        };
    }
}
