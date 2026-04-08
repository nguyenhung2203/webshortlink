namespace WebShortlink.Backend.Application.Analytics;

public sealed record AnalyticsOverviewDto(
    long TotalClicks,
    long UniqueClicks,
    long BotClicks,
    long ActiveLinks,
    IReadOnlyCollection<TrendPointDto> Trends,
    IReadOnlyCollection<TopLinkDto> TopLinks);

public sealed record TrendPointDto(string Bucket, long TotalClicks, long UniqueClicks, long BotClicks);

public sealed record TopLinkDto(Guid LinkId, string ShortUrl, long TotalClicks, long UniqueClicks, string Status);

public sealed record LinkAnalyticsDto(
    Guid LinkId,
    long TotalClicks,
    long UniqueClicks,
    long BotClicks,
    IReadOnlyCollection<TrendPointDto> Trends,
    IReadOnlyCollection<KeyValueMetricDto> TopCountries,
    IReadOnlyCollection<KeyValueMetricDto> TopDevices,
    IReadOnlyCollection<KeyValueMetricDto> TopReferrers,
    IReadOnlyCollection<KeyValueMetricDto> TopCampaigns);

public sealed record KeyValueMetricDto(string Label, long Value);
