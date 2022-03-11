using Microsoft.Extensions.Logging;

namespace BetaCensor.Workers
{
    public class QueueValidator<TRequest>
    {
        private readonly Func<TRequest, string?> _matcher;
        private readonly ILogger<QueueValidator<TRequest>> _logger;
        private readonly List<string> _cancelled = new();

        public QueueValidator(Func<TRequest, string?> matcher, ILogger<QueueValidator<TRequest>> logger)
        {
            _matcher = matcher;
            _logger = logger;
        }

        public void CancelRequests(IEnumerable<string> identifiers) {
            _cancelled.AddRange(identifiers);
        }

        public bool IsValid(TRequest request) {
            try {
            var match = _matcher(request);
            if (match is not null && _cancelled.Contains(match)) {
                _cancelled.Remove(match);
                _logger.LogInformation("Skipping cancelled request: " + match);
                return false;
            }
            return true;
            } catch {
                return true;
            }

        }
    }
}