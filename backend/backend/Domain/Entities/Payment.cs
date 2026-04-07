using WebShortlink.Backend.Domain.Common;
using WebShortlink.Backend.Domain.Enums;

namespace WebShortlink.Backend.Domain.Entities;

public class Payment : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string Provider { get; set; } = "Manual";
    public string? ProviderReference { get; set; }
    public DateTime? PaidAtUtc { get; set; }

    public Subscription Subscription { get; set; } = null!;
}
