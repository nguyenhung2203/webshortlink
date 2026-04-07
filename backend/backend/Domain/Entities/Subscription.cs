using WebShortlink.Backend.Domain.Common;
using WebShortlink.Backend.Domain.Enums;

namespace WebShortlink.Backend.Domain.Entities;

public class Subscription : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int PlanId { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pending;
    public DateTime StartAtUtc { get; set; }
    public DateTime EndAtUtc { get; set; }
    public bool AutoRenew { get; set; }

    public AppUser User { get; set; } = null!;
    public Plan Plan { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
