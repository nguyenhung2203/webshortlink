namespace WebShortlink.Backend.Application.Common;

public sealed class ClickTrackingMessage
{
    public Guid LinkId { get; init; }
    public DateTime ClickedAtUtc { get; init; }
    public string IpAddress { get; init; } = string.Empty;
    public string Host { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string Referrer { get; init; } = "direct";
    public string UserAgent { get; init; } = string.Empty;
    public int ResponseTimeMs { get; init; }
    public string EventStatus { get; init; } = string.Empty;
}
