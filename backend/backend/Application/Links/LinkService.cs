using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Application.Domains;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Links;

public sealed class LinkService
{
    private const string LinkLimitFeatureKey = "links.max_count";
    private const string DomainsFeatureKey = "domains.custom";
    private const string DefaultHost = "sho.rt";
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILinkCacheService _linkCacheService;
    private readonly IPlanCapabilityService _planCapabilityService;
    private readonly IPasswordHasher<Link> _passwordHasher;

    public LinkService(
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        ILinkCacheService linkCacheService,
        IPlanCapabilityService planCapabilityService,
        IPasswordHasher<Link> passwordHasher)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _linkCacheService = linkCacheService;
        _planCapabilityService = planCapabilityService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LinkDetailDto> CreateAsync(CreateLinkRequestDto request, CancellationToken cancellationToken)
    {
        ValidateLinkInput(request.OriginalUrl, request.ClickLimit);
        var current = _currentUserService.GetRequired();
        var limit = await _planCapabilityService.GetLimitAsync(current.UserId, LinkLimitFeatureKey, cancellationToken) ?? 100;
        var currentCount = await _dbContext.Links.CountAsync(x => x.UserId == current.UserId && !x.IsDeleted, cancellationToken);
        if (currentCount >= limit)
        {
            throw new AppException(ErrorCodes.PlanFeatureDenied, "Đã vượt hạn mức số lượng link của gói hiện tại.", StatusCodes.Status403Forbidden);
        }

        var domain = await ResolveOwnedDomainAsync(current.UserId, request.DomainId, cancellationToken);
        var slug = string.IsNullOrWhiteSpace(request.CustomSlug) ? GenerateSlug() : request.CustomSlug.Trim().ToLowerInvariant();
        var exists = await _dbContext.Links.AnyAsync(x => x.Slug == slug && x.DomainId == request.DomainId && !x.IsDeleted, cancellationToken);
        if (exists)
        {
            throw new AppException(ErrorCodes.Conflict, "Slug đã tồn tại.", StatusCodes.Status409Conflict);
        }

        var link = new Link
        {
            Id = Guid.NewGuid(),
            UserId = current.UserId,
            DomainId = domain?.Id,
            Slug = slug,
            OriginalUrl = request.OriginalUrl.Trim(),
            Description = request.Description?.Trim(),
            Tag = request.Tag?.Trim(),
            ExpiresAtUtc = request.ExpiresAtUtc,
            ClickLimit = request.ClickLimit,
            Status = LinkStatus.Active,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = current.UserId.ToString()
        };

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            link.PasswordHash = _passwordHasher.HashPassword(link, request.Password);
        }

        _dbContext.Links.Add(link);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await SetCacheAsync(link, domain?.Host, cancellationToken);

        return MapDetail(link, domain?.Host);
    }

    public async Task<IReadOnlyCollection<LinkListItemDto>> GetMineAsync(CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var items = await _dbContext.Links
            .AsNoTracking()
            .Where(x => x.UserId == current.UserId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new LinkListItemDto(
                x.Id,
                BuildShortUrl(x.Domain != null ? x.Domain.Host : DefaultHost, x.Slug),
                x.Slug,
                x.Domain != null ? x.Domain.Host : DefaultHost,
                x.OriginalUrl,
                x.Status.ToString(),
                x.Tag,
                x.TotalClicks,
                x.UniqueClicks,
                x.ClickEvents.LongCount(y => y.IsBot),
                x.CreatedAtUtc,
                x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<LinkDetailDto> GetMyDetailAsync(Guid linkId, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var link = await _dbContext.Links.AsNoTracking()
            .Include(x => x.Domain)
            .FirstOrDefaultAsync(x => x.Id == linkId && x.UserId == current.UserId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy link.", StatusCodes.Status404NotFound);
        return MapDetail(link, link.Domain?.Host);
    }

    public async Task<LinkDetailDto> UpdateAsync(Guid linkId, UpdateLinkRequestDto request, CancellationToken cancellationToken)
    {
        ValidateLinkInput(request.OriginalUrl, request.ClickLimit);
        var current = _currentUserService.GetRequired();
        var link = await _dbContext.Links
            .Include(x => x.Domain)
            .FirstOrDefaultAsync(x => x.Id == linkId && x.UserId == current.UserId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy link.", StatusCodes.Status404NotFound);

        var previousHost = link.Domain?.Host;
        var domain = await ResolveOwnedDomainAsync(current.UserId, request.DomainId, cancellationToken);
        if (link.DomainId != request.DomainId)
        {
            var slugExists = await _dbContext.Links.AnyAsync(
                x => x.Id != link.Id && x.Slug == link.Slug && x.DomainId == request.DomainId && !x.IsDeleted,
                cancellationToken);

            if (slugExists)
            {
                throw new AppException(ErrorCodes.Conflict, "Slug da ton tai tren domain duoc chon.", StatusCodes.Status409Conflict);
            }
        }

        link.DomainId = domain?.Id;
        link.OriginalUrl = request.OriginalUrl.Trim();
        link.Description = request.Description?.Trim();
        link.Tag = request.Tag?.Trim();
        link.ExpiresAtUtc = request.ExpiresAtUtc;
        link.ClickLimit = request.ClickLimit;
        link.UpdatedAtUtc = DateTime.UtcNow;
        link.UpdatedByUserId = current.UserId.ToString();
        link.PasswordHash = string.IsNullOrWhiteSpace(request.Password) ? null : _passwordHasher.HashPassword(link, request.Password);

        await _dbContext.SaveChangesAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(previousHost))
        {
            await _linkCacheService.RemoveAsync(previousHost, link.Slug, cancellationToken);
        }

        await SetCacheAsync(link, domain?.Host, cancellationToken);

        return MapDetail(link, domain?.Host);
    }

    public async Task<MessageResponseDto> ToggleAsync(Guid linkId, ToggleLinkStatusRequestDto request, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var link = await _dbContext.Links
            .Include(x => x.Domain)
            .FirstOrDefaultAsync(x => x.Id == linkId && x.UserId == current.UserId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy link.", StatusCodes.Status404NotFound);

        link.Status = request.IsActive ? LinkStatus.Active : LinkStatus.Paused;
        link.UpdatedAtUtc = DateTime.UtcNow;
        link.UpdatedByUserId = current.UserId.ToString();
        await _dbContext.SaveChangesAsync(cancellationToken);
        await SetCacheAsync(link, link.Domain?.Host, cancellationToken);

        return new MessageResponseDto("Cập nhật trạng thái link thành công.");
    }

    public async Task<MessageResponseDto> DeleteAsync(Guid linkId, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var link = await _dbContext.Links
            .Include(x => x.Domain)
            .FirstOrDefaultAsync(x => x.Id == linkId && x.UserId == current.UserId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy link.", StatusCodes.Status404NotFound);

        link.IsDeleted = true;
        link.DeletedAtUtc = DateTime.UtcNow;
        link.DeletedByUserId = current.UserId.ToString();
        link.Status = LinkStatus.Paused;
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _linkCacheService.RemoveAsync(link.Domain?.Host ?? DefaultHost, link.Slug, cancellationToken);

        return new MessageResponseDto("Đã xóa link.");
    }

    private async Task<CustomDomain?> ResolveOwnedDomainAsync(Guid userId, Guid? domainId, CancellationToken cancellationToken)
    {
        if (!domainId.HasValue)
        {
            return null;
        }

        await _planCapabilityService.EnsureFeatureEnabledAsync(userId, DomainsFeatureKey, cancellationToken);
        var domain = await _dbContext.Domains.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == domainId.Value && x.UserId == userId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Khong tim thay domain.", StatusCodes.Status404NotFound);

        if (!domain.IsVerified)
        {
            throw new AppException(ErrorCodes.DomainNotVerified, "Domain chua duoc xac minh.", StatusCodes.Status400BadRequest);
        }

        return domain;
    }

    private async Task SetCacheAsync(Link link, string? host, CancellationToken cancellationToken)
    {
        await _linkCacheService.SetAsync(new CachedLinkDto(
            link.Id,
            link.UserId,
            host ?? DefaultHost,
            link.Slug,
            link.OriginalUrl,
            link.Status.ToString(),
            link.ExpiresAtUtc,
            link.ClickLimit,
            link.PasswordHash,
            link.TotalClicks), cancellationToken);
    }

    private static void ValidateLinkInput(string originalUrl, int? clickLimit)
    {
        if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out _))
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Original URL không hợp lệ.");
        }

        if (clickLimit.HasValue && clickLimit <= 0)
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Click limit phải lớn hơn 0.");
        }
    }

    private static string GenerateSlug()
    {
        const string chars = "abcdefghjkmnpqrstuvwxyz23456789";
        return new string(Enumerable.Range(0, 7).Select(_ => chars[Random.Shared.Next(chars.Length)]).ToArray());
    }

    private static string BuildShortUrl(string host, string slug) => $"https://{host}/{slug}";

    private static LinkDetailDto MapDetail(Link link, string? host)
    {
        var resolvedHost = host ?? DefaultHost;
        return new LinkDetailDto(
            link.Id,
            BuildShortUrl(resolvedHost, link.Slug),
            link.Slug,
            resolvedHost,
            link.OriginalUrl,
            link.Status.ToString(),
            link.Description,
            link.Tag,
            link.ExpiresAtUtc,
            link.ClickLimit,
            !string.IsNullOrWhiteSpace(link.PasswordHash),
            link.TotalClicks,
            link.UniqueClicks,
            link.CreatedAtUtc,
            link.UpdatedAtUtc);
    }
}
