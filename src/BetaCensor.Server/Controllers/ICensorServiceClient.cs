using BetaCensor.Core.Messaging;

namespace BetaCensor.Server.Controllers {
    public interface ICensorServiceClient {
        Task HandleCensoredImage(CensorImageResponse response);
        Task OnRequestUpdate(string requestId, string? stateMessage);
        Task OnCensoringError(string requestId, string? errorMessage);
    }
}