using Microsoft.AspNetCore.Identity;
using WebShortlink.Backend.Domain.Enums;

namespace WebShortlink.Backend.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public UserAccountStatus AccountStatus { get; set; } = UserAccountStatus.PendingVerification;
    public int CurrentPlanId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<RefreshSession> RefreshSessions { get; set; } = new List<RefreshSession>();
    public ICollection<Link> Links { get; set; } = new List<Link>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<CustomDomain> Domains { get; set; } = new List<CustomDomain>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public ICollection<AbuseReport> AbuseReports { get; set; } = new List<AbuseReport>();
}
