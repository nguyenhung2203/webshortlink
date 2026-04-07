using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Links;

public sealed class LinkRuleService
{
    private const string AdvancedTargetingFeatureKey = "targeting.advanced";

    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPlanCapabilityService _planCapabilityService;

    public LinkRuleService(
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IPlanCapabilityService planCapabilityService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _planCapabilityService = planCapabilityService;
    }

    public async Task<IReadOnlyCollection<LinkRuleItemDto>> GetRulesAsync(Guid linkId, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        await EnsureOwnedLinkAsync(current.UserId, linkId, cancellationToken);

        return await _dbContext.LinkRules.AsNoTracking()
            .Where(x => x.LinkId == linkId)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.CreatedAtUtc)
            .Select(x => new LinkRuleItemDto(x.Id, x.RuleType.ToString(), x.RuleValue, x.TargetUrl, x.Priority, x.IsActive, x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<LinkRuleItemDto> CreateRuleAsync(Guid linkId, CreateLinkRuleRequestDto request, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        await _planCapabilityService.EnsureFeatureEnabledAsync(current.UserId, AdvancedTargetingFeatureKey, cancellationToken);
        await EnsureOwnedLinkAsync(current.UserId, linkId, cancellationToken);

        if (!Enum.TryParse<LinkRuleType>(request.RuleType, true, out var ruleType))
        {
            throw new AppException(ErrorCodes.InvalidRule, "Rule type khong hop le.");
        }

        if (!Uri.TryCreate(request.TargetUrl, UriKind.Absolute, out _))
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Target URL khong hop le.");
        }

        if (request.Priority < 0)
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Priority khong hop le.");
        }

        if (ruleType != LinkRuleType.Rotation && string.IsNullOrWhiteSpace(request.RuleValue))
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Rule value khong duoc de trong.");
        }

        var rule = new LinkRule
        {
            Id = Guid.NewGuid(),
            LinkId = linkId,
            RuleType = ruleType,
            RuleValue = request.RuleValue?.Trim() ?? string.Empty,
            TargetUrl = request.TargetUrl.Trim(),
            Priority = request.Priority,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = current.UserId.ToString()
        };

        _dbContext.LinkRules.Add(rule);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new LinkRuleItemDto(rule.Id, rule.RuleType.ToString(), rule.RuleValue, rule.TargetUrl, rule.Priority, rule.IsActive, rule.CreatedAtUtc);
    }

    public async Task<MessageResponseDto> DeleteRuleAsync(Guid linkId, Guid ruleId, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        await EnsureOwnedLinkAsync(current.UserId, linkId, cancellationToken);

        var rule = await _dbContext.LinkRules.FirstOrDefaultAsync(x => x.Id == ruleId && x.LinkId == linkId, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Khong tim thay rule.", StatusCodes.Status404NotFound);

        _dbContext.LinkRules.Remove(rule);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new MessageResponseDto("Da xoa targeting rule.");
    }

    private async Task EnsureOwnedLinkAsync(Guid userId, Guid linkId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Links.AnyAsync(x => x.Id == linkId && x.UserId == userId && !x.IsDeleted, cancellationToken);
        if (!exists)
        {
            throw new AppException(ErrorCodes.NotFound, "Khong tim thay link.", StatusCodes.Status404NotFound);
        }
    }
}
