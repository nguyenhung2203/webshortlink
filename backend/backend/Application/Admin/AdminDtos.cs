using WebShortlink.Backend.Application.Analytics;

namespace WebShortlink.Backend.Application.Admin;

using WebShortlink.Backend.Application.Analytics;

public sealed record AdminOverviewDto(
    BusinessOverviewDto Business,
    ProductOverviewDto Product,
    OperationsOverviewDto Operations);

public sealed record BusinessOverviewDto(long TotalUsers, long PaidUsers, decimal ConversionRate, decimal MonthlyRevenue);

public sealed record ProductOverviewDto(long TotalLinks, long ActiveLinks, long TotalClicks, long UniqueClicks);

public sealed record OperationsOverviewDto(long BotClicks, long SuspiciousClicks, decimal ErrorRate, int QueueLagSeconds, int RedirectLatencyP95Ms);

public sealed record AdminUserListItemDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    string AccountStatus,
    string PlanName,
    long LinksCount,
    long TotalClicks,
    DateTime CreatedAtUtc,
    DateTime? LastLoginAtUtc);

public sealed record AdminUserDetailDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    string AccountStatus,
    int CurrentPlanId,
    string CurrentPlanName,
    DateTime CreatedAtUtc,
    DateTime? LastLoginAtUtc,
    IReadOnlyCollection<AdminUserLinkDto> Links);

public sealed record AdminUserLinkDto(Guid LinkId, string ShortUrl, string Status, long TotalClicks);

public sealed record ChangeUserStatusRequestDto(string Status);

public sealed record ChangeUserPlanRequestDto(int PlanId);

public sealed record AdminLinkListItemDto(
    Guid Id,
    string ShortUrl,
    string Slug,
    string OriginalUrl,
    string Status,
    string OwnerEmail,
    long TotalClicks,
    long BotClicks,
    int AbuseScore,
    DateTime CreatedAtUtc);

public sealed record AdminLinkDetailDto(
    Guid Id,
    string ShortUrl,
    string Slug,
    string OriginalUrl,
    string Status,
    string OwnerEmail,
    long TotalClicks,
    long UniqueClicks,
    long BotClicks,
    IReadOnlyCollection<KeyValueMetricDto> RecentCountries);
