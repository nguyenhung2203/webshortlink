using WebShortlink.Backend.Domain.Enums;

namespace WebShortlink.Backend.Application.Abstractions;

public interface IAuditLogService
{
    Task WriteAsync(
        AuditActorType actorType,
        string action,
        string resourceType,
        string? resourceId,
        Guid? actorUserId,
        string? ipAddress,
        object? metadata,
        CancellationToken cancellationToken = default);
}
