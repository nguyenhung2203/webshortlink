using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Infrastructure.Services;

public sealed class PlanCapabilityService : IPlanCapabilityService
{
    private readonly ApplicationDbContext _dbContext;

    public PlanCapabilityService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task EnsureFeatureEnabledAsync(Guid userId, string featureKey, CancellationToken cancellationToken = default)
    {
        var feature = await GetFeatureAsync(userId, featureKey, cancellationToken);
        if (feature is null || !feature.IsEnabled)
        {
           throw new AppException(ErrorCodes.PlanFeatureDenied, "Gói hiện tại không hỗ trợ tính năng này.", StatusCodes.Status403Forbidden);
        }
    }

    public async Task<int?> GetLimitAsync(Guid userId, string featureKey, CancellationToken cancellationToken = default)
    {
        var feature = await GetFeatureAsync(userId, featureKey, cancellationToken);
        return feature?.LimitValue;
    }

    private async Task<Domain.Entities.PlanFeature?> GetFeatureAsync(Guid userId, string featureKey, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.AsNoTracking().FirstAsync(x => x.Id == userId, cancellationToken);
        return await _dbContext.PlanFeatures
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PlanId == user.CurrentPlanId && x.FeatureKey == featureKey, cancellationToken);
    }
}
