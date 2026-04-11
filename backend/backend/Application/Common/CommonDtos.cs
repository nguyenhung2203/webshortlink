namespace WebShortlink.Backend.Application.Common;

public sealed record PlanFeatureInfo(string Title, string? Hint, bool IsEnabled);

public sealed record PlanDto(int Id, string Code, string Name, decimal MonthlyPrice, bool IsActive, IReadOnlyCollection<PlanFeatureInfo> Features);

public sealed record SubscriptionDto(Guid Id, int PlanId, string PlanName, string Status, DateTime StartAtUtc, DateTime EndAtUtc);

public sealed record PaymentHistoryItemDto(Guid Id, decimal Amount, string Currency, string Status, DateTime? PaidAtUtc);

public sealed record UpdateProfileRequestDto(string FullName);

public sealed record ChangePasswordRequestDto(string CurrentPassword, string NewPassword, string ConfirmPassword);

public sealed record CurrentSessionDto(Guid UserId, string Email, string FullName, string Role, int CurrentPlanId, string AccountStatus);

public sealed record PublicRedirectAccessRequestDto(string Password);

public sealed record PublicRedirectAccessResponseDto(string RedirectUrl);

public sealed record PublicWrapperRenderDto(
    string WrapperUrl,
    string ContinueUrl,
    string Host,
    string Slug,
    string OriginalUrl,
    string RedirectMode,
    int DelaySeconds,
    string Title,
    string Description,
    string? ImageUrl,
    string ButtonText,
    string WarningText,
    string Theme,
    string? BrandName,
    string? BrandLogoUrl,
    string? CtaTitle,
    string? CtaDescription,
    string? CtaButtonText,
    string? CtaButtonUrl);

public sealed record OgLinkDataDto(
    string OriginalUrl,
    string? OgTitle,
    string? OgDescription,
    string? OgImageUrl,
    string Host,
    string Slug);

public sealed record MessageResponseDto(string Message, string? Code = null);

public sealed class PaginatedResponseDto<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
    public int PageIndex { get; init; }
    public int PageSize { get; init; }

    public PaginatedResponseDto() { }
    public PaginatedResponseDto(IReadOnlyCollection<T> items, int totalCount, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}
