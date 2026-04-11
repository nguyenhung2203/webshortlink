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

public sealed record AdminReportsDto(
    long TotalUsers,
    long TotalLinks,
    long TotalClicks,
    long ActiveSubscriptions,
    IReadOnlyCollection<AdminPlanBreakdownDto> PlanBreakdown);

public sealed record AdminPlanBreakdownDto(
    int PlanId,
    string PlanName,
    long Count,
    decimal Percent);

// Plan management DTOs
public sealed record AdminPlanFeatureDto(
    int Id,
    string FeatureKey,
    bool IsEnabled,
    int? LimitValue,
    string? FeatureValue);

public sealed record AdminPlanDetailDto(
    int Id,
    string Code,
    string Name,
    decimal MonthlyPrice,
    bool IsActive,
    IReadOnlyCollection<AdminPlanFeatureDto> Features);

public sealed record AdminUpdateFeatureRequestDto(
    bool IsEnabled,
    int? LimitValue,
    string? FeatureValue);

public sealed record AdminFeatureLabelDto(
    string FeatureKey,
    string Label,
    string Description,
    string FeatureType);  // "toggle" or "number"

public sealed record AdminSaveFeatureLabelDto(
    string FeatureKey,
    string Label,
    string Description,
    string FeatureType);

public sealed record AdminSecurityDto(
    long FailedLoginsToday,
    long SuspiciousClicks,
    long LockedAccounts,
    IReadOnlyCollection<AdminSystemHealthItemDto> SystemHealth);

public sealed record AdminSystemHealthItemDto(
    string Name,
    string Status,
    string Value);

public sealed record AdminUserListItemDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    string Status,
    string PlanName,
    long TotalLinks,
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
    string UserEmail,
    long TotalClicks,
    long BotClicks,
    int? HighestRiskScore,
    DateTime CreatedAtUtc);

public sealed record AdminLinkDetailDto(
    Guid Id,
    string ShortUrl,
    string Slug,
    string OriginalUrl,
    string Status,
    string UserEmail,
    long TotalClicks,
    long UniqueClicks,
    long BotClicks,
    IReadOnlyCollection<KeyValueMetricDto> RecentCountries);

public sealed record AdminAuditLogItemDto(
    Guid Id,
    string Action,
    string? ResourceType,
    string? ResourceId,
    string ActorType,
    DateTime CreatedAtUtc);

public sealed record AdminPaymentListItemDto(
    Guid Id,
    string UserEmail,
    string PlanName,
    decimal Amount,
    string Status,
    string Provider,
    DateTime CreatedAtUtc,
    DateTime? PaidAtUtc);

public sealed record AdminVerifyDomainRequestDto(string? AdminFeedback);

public sealed record AdminDomainListItemDto(
    Guid Id,
    string Host,
    bool IsVerified,
    string? AdminFeedback,
    DateTime? VerifiedAtUtc,
    DateTime CreatedAtUtc,
    string UserEmail,
    long LinksCount,
    bool IsGlobal,
    DateTime? ExpiredAtUtc,
    string? UserNotes,
    bool IsDefault = false);

public sealed record AdminDomainDnsCheckResultDto(
    bool Verified,
    string Host,
    string Message);

public sealed record AdminCreateDomainForUserRequestDto(Guid? UserId, string Host, bool IsGlobal, DateTime? ExpiredAtUtc = null, string? UserNotes = null);

public sealed record AdminSaveSettingRequestDto(string Key, string Value, string? GroupName);
