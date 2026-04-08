namespace WebShortlink.Backend.Application.Billing;

public sealed record UpgradeSubscriptionRequestDto(int PlanId);

public sealed record UpgradeSubscriptionResponseDto(
    Guid SubscriptionId,
    int PlanId,
    string PlanCode,
    string PlanName,
    decimal AmountCharged,
    string Currency,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    string Message);

public sealed record PaymentHistoryDto(
    Guid Id,
    string PlanName,
    decimal Amount,
    string Currency,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? PaidAtUtc);
