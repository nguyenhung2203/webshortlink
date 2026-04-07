using System.Collections.Concurrent;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;

namespace WebShortlink.Backend.Infrastructure.Queue;

public sealed class InMemoryAnalyticsQueue : IAnalyticsQueue
{
    private readonly ConcurrentQueue<ClickTrackingMessage> _queue = new();

    public Task EnqueueAsync(ClickTrackingMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _queue.Enqueue(message);
        return Task.CompletedTask;
    }

    public Task<ClickTrackingMessage?> DequeueAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_queue.TryDequeue(out var message) ? message : null);
    }
}
