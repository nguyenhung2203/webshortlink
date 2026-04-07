using WebShortlink.Backend.Application.Common;

namespace WebShortlink.Backend.Application.Abstractions;

public interface IAnalyticsQueue
{
    Task EnqueueAsync(ClickTrackingMessage message, CancellationToken cancellationToken = default);
    Task<ClickTrackingMessage?> DequeueAsync(CancellationToken cancellationToken = default);
}
