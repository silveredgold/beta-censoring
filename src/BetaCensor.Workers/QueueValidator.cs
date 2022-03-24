using Microsoft.Extensions.Logging;

namespace BetaCensor.Workers
{
    public class QueueValidator<TRequest>
    {
        private readonly Func<TRequest, string?> _matcher;
        private readonly ILogger<QueueValidator<TRequest>> _logger;
        private readonly List<string> _cancelled = new();
        private List<KeyValuePair<Func<TRequest, string?>, Predicate<string?>>> _filters = new();

        public QueueValidator(Func<TRequest, string?> matcher, ILogger<QueueValidator<TRequest>> logger)
        {
            _matcher = matcher;
            _logger = logger;
        }

        public QueueValidator<TRequest> AddFilter(Func<TRequest, string?> matcher, Predicate<string?> filter) {
            _filters.Add(new KeyValuePair<Func<TRequest, string?>, Predicate<string?>>(matcher, filter));
            return this;
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
            } catch (Exception e) {
                _logger.LogWarning(e, "Error encountered checking queue item state");
                return true;
            }

        }
    }
}