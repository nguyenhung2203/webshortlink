using WebShortlink.Backend.Domain.Common;

namespace WebShortlink.Backend.Domain.Entities;

public class LinkDailyStat : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid LinkId { get; set; }
    public DateOnly StatDate { get; set; }
    public long TotalClicks { get; set; }
    public long UniqueClicks { get; set; }
    public long BotClicks { get; set; }
    public long SuspiciousClicks { get; set; }

    public Link Link { get; set; } = null!;
}
