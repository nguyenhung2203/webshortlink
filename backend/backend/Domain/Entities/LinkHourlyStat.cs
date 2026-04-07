using WebShortlink.Backend.Domain.Common;

namespace WebShortlink.Backend.Domain.Entities;

public class LinkHourlyStat : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid LinkId { get; set; }
    public DateTime HourBucketUtc { get; set; }
    public long TotalClicks { get; set; }
    public long UniqueClicks { get; set; }
    public long BotClicks { get; set; }

    public Link Link { get; set; } = null!;
}
