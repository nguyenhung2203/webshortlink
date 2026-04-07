using WebShortlink.Backend.Domain.Common;

namespace WebShortlink.Backend.Domain.Entities;

public class AbuseReport : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid LinkId { get; set; }
    public Guid? ReportedByUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }

    public Link Link { get; set; } = null!;
    public AppUser? ReportedByUser { get; set; }
}
