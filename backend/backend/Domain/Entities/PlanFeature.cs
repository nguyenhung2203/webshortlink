using WebShortlink.Backend.Domain.Common;

namespace WebShortlink.Backend.Domain.Entities;

public class PlanFeature : AuditableEntity
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public string FeatureKey { get; set; } = string.Empty;
    public string? FeatureValue { get; set; }
    public int? LimitValue { get; set; }
    public bool IsEnabled { get; set; }

    public Plan Plan { get; set; } = null!;
}
