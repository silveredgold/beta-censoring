using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace BetaCensor.Workers {
    public class BackgroundQueue<T> : IAsyncBackgroundQueue<T> where T : class {
        public BackgroundQueue(ILogger<BackgroundQueue<T>> logger, QueueValidator<T>? manager = null)
        {
            this._logger = logger;
            this._manager = manager;
        }

        public readonly Channel<T> _items = Channel.CreateUnbounded<T>();
        private readonly ILogger<BackgroundQueue<T>> _logger;
        private readonly QueueValidator<T>? _manager;

        public async Task Enqueue(T item) {
            if (item == null) throw new ArgumentNullException(nameof(item));
            await _items.Writer.WriteAsync(item);
        }

        public async Task<T?> Dequeue(CancellationToken cancellationToken) {
            if (_manager != null) {
                T? validItem = null;
                while (validItem == null)
                {
                    var current = await _items.Reader.ReadAsync(cancellationToken);
                    if (_manager.IsValid(current)) {
                        validItem = current;
                    }
                }
                return validItem;
            } else {
                var item = await _items.Reader.ReadAsync(cancellationToken);
                return item;
            }
        }

        public int? GetItemCount() {
            return _items.Reader.CanCount ? _items.Reader.Count : null;
        }
    }
}