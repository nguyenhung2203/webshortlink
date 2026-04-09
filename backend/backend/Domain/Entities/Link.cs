using WebShortlink.Backend.Domain.Common;
using WebShortlink.Backend.Domain.Enums;

namespace WebShortlink.Backend.Domain.Entities;

public class Link : SoftDeleteEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? DomainId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Tag { get; set; }
    public LinkStatus Status { get; set; } = LinkStatus.Active;
    public DateTime? ExpiresAtUtc { get; set; }
    public int? ClickLimit { get; set; }
    public string? PasswordHash { get; set; }
    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? OgImageUrl { get; set; }
    public long TotalClicks { get; set; }
    public long UniqueClicks { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public AppUser User { get; set; } = null!;
    public CustomDomain? Domain { get; set; }
    public ICollection<LinkRule> Rules { get; set; } = new List<LinkRule>();
    public ICollection<ClickEvent> ClickEvents { get; set; } = new List<ClickEvent>();
    public ICollection<LinkDailyStat> DailyStats { get; set; } = new List<LinkDailyStat>();
    public ICollection<LinkHourlyStat> HourlyStats { get; set; } = new List<LinkHourlyStat>();
    public ICollection<AbuseReport> AbuseReports { get; set; } = new List<AbuseReport>();
}
