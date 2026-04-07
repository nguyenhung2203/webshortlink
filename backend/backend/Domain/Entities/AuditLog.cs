using WebShortlink.Backend.Domain.Common;
using WebShortlink.Backend.Domain.Enums;

namespace WebShortlink.Backend.Domain.Entities;

public class AuditLog : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid? ActorUserId { get; set; }
    public AuditActorType ActorType { get; set; }
    public string Action { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string? ResourceId { get; set; }
    public string? MetadataJson { get; set; }
    public string? IpAddress { get; set; }

    public AppUser? ActorUser { get; set; }
}
