using System.Threading.Channels;

namespace BetaCensor.Workers
{
    public class BackgroundQueue<T> : IAsyncBackgroundQueue<T> where T : class
    {
        private readonly Channel<T> _items = Channel.CreateUnbounded<T>();

        public async Task Enqueue(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            await _items.Writer.WriteAsync(item);
        }

        public async Task<T?> Dequeue(CancellationToken cancellationToken)
        {
            var item = await _items.Reader.ReadAsync(cancellationToken);
            return item;
        }
    }
}