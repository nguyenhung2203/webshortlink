namespace WebShortlink.Backend.Application.Abstractions;

using WebShortlink.Backend.Application.Common;

public interface IAnalyticsQueue
{
    Task EnqueueAsync(ClickTrackingMessage message, CancellationToken cancellationToken = default);
    Task<ClickTrackingMessage?> DequeueAsync(CancellationToken cancellationToken = default);
}
