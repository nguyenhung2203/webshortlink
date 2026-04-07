namespace WebShortlink.Backend.Application.Common;

public sealed record PlanDto(int Id, string Code, string Name, decimal MonthlyPrice, bool IsActive);

public sealed record SubscriptionDto(Guid Id, int PlanId, string PlanName, string Status, DateTime StartAtUtc, DateTime EndAtUtc);

public sealed record PaymentHistoryItemDto(Guid Id, decimal Amount, string Currency, string Status, DateTime? PaidAtUtc);

public sealed record UpdateProfileRequestDto(string FullName);

public sealed record ChangePasswordRequestDto(string CurrentPassword, string NewPassword, string ConfirmPassword);

public sealed record CurrentSessionDto(Guid UserId, string Email, string FullName, string Role, int CurrentPlanId, string AccountStatus);

public sealed record PublicRedirectAccessRequestDto(string Password);

public sealed record PublicRedirectAccessResponseDto(string RedirectUrl);

public sealed record MessageResponseDto(string Message, string? Code = null);
