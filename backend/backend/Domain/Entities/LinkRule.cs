using WebShortlink.Backend.Domain.Common;
using WebShortlink.Backend.Domain.Enums;

namespace WebShortlink.Backend.Domain.Entities;

public class LinkRule : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid LinkId { get; set; }
    public LinkRuleType RuleType { get; set; }
    public string RuleValue { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;

    public Link Link { get; set; } = null!;
}
