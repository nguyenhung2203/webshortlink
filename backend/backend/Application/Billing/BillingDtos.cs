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
