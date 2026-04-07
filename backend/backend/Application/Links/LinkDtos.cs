namespace WebShortlink.Backend.Application.Links;

public sealed record CreateLinkRequestDto(
    string OriginalUrl,
    string? CustomSlug,
    string? Description,
    string? Tag,
    DateTime? ExpiresAtUtc,
    int? ClickLimit,
    string? Password);

public sealed record UpdateLinkRequestDto(
    string OriginalUrl,
    string? Description,
    string? Tag,
    DateTime? ExpiresAtUtc,
    int? ClickLimit,
    string? Password);

public sealed record ToggleLinkStatusRequestDto(bool IsActive);

public sealed record LinkListItemDto(
    Guid Id,
    string ShortUrl,
    string Slug,
    string OriginalUrl,
    string Status,
    string? Tag,
    long TotalClicks,
    long UniqueClicks,
    long BotClicks,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record LinkDetailDto(
    Guid Id,
    string ShortUrl,
    string Slug,
    string OriginalUrl,
    string Status,
    string? Description,
    string? Tag,
    DateTime? ExpiresAtUtc,
    int? ClickLimit,
    bool HasPassword,
    long TotalClicks,
    long UniqueClicks,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record CachedLinkDto(
    Guid Id,
    Guid UserId,
    string Host,
    string Slug,
    string OriginalUrl,
    string Status,
    DateTime? ExpiresAtUtc,
    int? ClickLimit,
    string? PasswordHash,
    long TotalClicks);
