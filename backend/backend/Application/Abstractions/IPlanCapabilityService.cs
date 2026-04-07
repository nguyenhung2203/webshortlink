namespace WebShortlink.Backend.Application.Abstractions;

public interface IPlanCapabilityService
{
    Task EnsureFeatureEnabledAsync(Guid userId, string featureKey, CancellationToken cancellationToken = default);
    Task<int?> GetLimitAsync(Guid userId, string featureKey, CancellationToken cancellationToken = default);
}
