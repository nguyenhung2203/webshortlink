using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Application.Domains;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;
using WebShortlink.Backend.Infrastructure.Options;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Links;

public sealed class LinkService
{
    private const string LinkLimitFeatureKey = "links.max_count";
    private const string CustomSlugFeatureKey = "links.custom_slug";
    private const string DomainsFeatureKey = "domains.custom";
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILinkCacheService _linkCacheService;
    private readonly IPlanCapabilityService _planCapabilityService;
    private readonly IPasswordHasher<Link> _passwordHasher;
    private readonly string _defaultHost;

    public LinkService(
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        ILinkCacheService linkCacheService,
        IPlanCapabilityService planCapabilityService,
        IPasswordHasher<Link> passwordHasher,
        IOptions<AppOptions> appOptions)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _linkCacheService = linkCacheService;
        _planCapabilityService = planCapabilityService;
        _passwordHasher = passwordHasher;
        _defaultHost = appOptions.Value.DefaultDomain;
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
        var hasCustomSlug = !string.IsNullOrWhiteSpace(request.CustomSlug);
        if (hasCustomSlug)
        {
            await _planCapabilityService.EnsureFeatureEnabledAsync(current.UserId, CustomSlugFeatureKey, cancellationToken);
        }

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
            await _planCapabilityService.EnsureFeatureEnabledAsync(current.UserId, "links.password_protection", cancellationToken);
            link.PasswordHash = _passwordHasher.HashPassword(link, request.Password);
        }
        
        if (request.ExpiresAtUtc.HasValue)
        {
            await _planCapabilityService.EnsureFeatureEnabledAsync(current.UserId, "links.expiration", cancellationToken);
        }

        if (request.ClickLimit.HasValue)
        {
            await _planCapabilityService.EnsureFeatureEnabledAsync(current.UserId, "links.click_limit", cancellationToken);
        }

        _dbContext.Links.Add(link);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var defaultHost = await GetSystemDefaultHostAsync(cancellationToken);
        await SetCacheAsync(link, domain?.Host, defaultHost, cancellationToken);

        return MapDetail(link, domain?.Host, defaultHost);
    }

    public async Task<PaginatedResponseDto<LinkListItemDto>> GetMineAsync(UserLinkFilterDto filter, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var defaultHost = await GetSystemDefaultHostAsync(cancellationToken);
        
        var items = await _dbContext.Links
            .AsNoTracking()
            .Include(x => x.Domain)
            .Where(x => x.UserId == current.UserId && !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(x => 
                x.Slug.ToLower().Contains(search) || 
                x.OriginalUrl.ToLower().Contains(search) || 
                (!string.IsNullOrEmpty(x.Description) && x.Description.ToLower().Contains(search)) ||
                (!string.IsNullOrEmpty(x.Tag) && x.Tag.ToLower().Contains(search))
            );
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (Enum.TryParse<LinkStatus>(filter.Status, true, out var statusEnum))
            {
                query = query.Where(x => x.Status == statusEnum);
            }
        }

        if (!string.IsNullOrWhiteSpace(filter.Tag))
        {
            var tag = filter.Tag.Trim().ToLower();
            query = query.Where(x => !string.IsNullOrEmpty(x.Tag) && x.Tag.ToLower().Contains(tag));
        }

        if (!string.IsNullOrWhiteSpace(filter.Description))
        {
            var desc = filter.Description.Trim().ToLower();
            query = query.Where(x => !string.IsNullOrEmpty(x.Description) && x.Description.ToLower().Contains(desc));
        }

        if (filter.StartDate.HasValue)
        {
            var start = filter.StartDate.Value.Date;
            query = query.Where(x => x.CreatedAtUtc >= start);
        }

        if (filter.EndDate.HasValue)
        {
            var end = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(x => x.CreatedAtUtc <= end);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        if (filter.SortBy?.ToLower() == "clicks")
        {
            query = query.OrderByDescending(x => x.TotalClicks);
        }
        else
        {
            query = query.OrderByDescending(x => x.CreatedAtUtc);
        }

        var items = await query
            .Skip((filter.PageIndex - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(x => new LinkListItemDto(
                x.Id,
                BuildShortUrl(x.Domain != null ? x.Domain.Host : defaultHost, x.Slug),
                x.Slug,
                x.Domain != null ? x.Domain.Host : defaultHost,
                x.OriginalUrl,
                x.Status.ToString(),
                x.Tag,
                x.Description,
                x.TotalClicks,
                x.UniqueClicks,
                x.ClickEvents.LongCount(y => y.IsBot),
                x.CreatedAtUtc,
                x.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return new PaginatedResponseDto<LinkListItemDto>(items, totalCount, filter.PageIndex, filter.PageSize);
    }

    public async Task<LinkDetailDto> GetMyDetailAsync(Guid linkId, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var link = await _dbContext.Links.AsNoTracking()
            .Include(x => x.Domain)
            .FirstOrDefaultAsync(x => x.Id == linkId && x.UserId == current.UserId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy link.", StatusCodes.Status404NotFound);
            
        var defaultHost = await GetSystemDefaultHostAsync(cancellationToken);
        return MapDetail(link, link.Domain?.Host, defaultHost);
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
        if (request.ExpiresAtUtc.HasValue && request.ExpiresAtUtc != link.ExpiresAtUtc)
        {
            await _planCapabilityService.EnsureFeatureEnabledAsync(current.UserId, "links.expiration", cancellationToken);
        }
        link.ExpiresAtUtc = request.ExpiresAtUtc;

        if (request.ClickLimit.HasValue && request.ClickLimit != link.ClickLimit)
        {
            await _planCapabilityService.EnsureFeatureEnabledAsync(current.UserId, "links.click_limit", cancellationToken);
        }
        link.ClickLimit = request.ClickLimit;

        link.UpdatedAtUtc = DateTime.UtcNow;
        link.UpdatedByUserId = current.UserId.ToString();

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            await _planCapabilityService.EnsureFeatureEnabledAsync(current.UserId, "links.password_protection", cancellationToken);
            link.PasswordHash = _passwordHasher.HashPassword(link, request.Password);
        }
        else if (request.Password == "")
        {
            link.PasswordHash = null;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(previousHost))
        {
            await _linkCacheService.RemoveAsync(previousHost, link.Slug, cancellationToken);
        }

        var defaultHost = await GetSystemDefaultHostAsync(cancellationToken);
        await SetCacheAsync(link, domain?.Host, defaultHost, cancellationToken);

        return MapDetail(link, domain?.Host, defaultHost);
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
        var defaultHost = await GetSystemDefaultHostAsync(cancellationToken);
        await SetCacheAsync(link, link.Domain?.Host, defaultHost, cancellationToken);

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
        var defaultHost = await GetSystemDefaultHostAsync(cancellationToken);
        await _linkCacheService.RemoveAsync(link.Domain?.Host ?? defaultHost, link.Slug, cancellationToken);

        return new MessageResponseDto("Đã xóa link.");
    }

    private async Task<CustomDomain?> ResolveOwnedDomainAsync(Guid userId, Guid? domainId, CancellationToken cancellationToken)
    {
        if (!domainId.HasValue)
        {
            return null;
        }

        var domain = await _dbContext.Domains.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == domainId.Value && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy domain.", StatusCodes.Status404NotFound);

        // Nêú không phải Global Domain thì user bắt buộc phải là chủ sở hữu, và phải có gói premium.
        if (!domain.IsGlobal)
        {
            if (domain.UserId != userId)
            {
                throw new AppException(ErrorCodes.NotFound, "Không tìm thấy domain.", StatusCodes.Status404NotFound);
            }
            await _planCapabilityService.EnsureFeatureEnabledAsync(userId, DomainsFeatureKey, cancellationToken);
        }

        if (!domain.IsVerified)
        {
            throw new AppException(ErrorCodes.DomainNotVerified, "Domain chưa được xác minh.", StatusCodes.Status400BadRequest);
        }

        return domain;
    }

    private async Task<string> GetSystemDefaultHostAsync(CancellationToken cancellationToken = default)
    {
        var setting = await _dbContext.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SettingKey == "DefaultDomain", cancellationToken);
        return setting?.SettingValue ?? _defaultHost;
    }

    private async Task SetCacheAsync(Link link, string? host, string defaultHost, CancellationToken cancellationToken)
    {
        await _linkCacheService.SetAsync(new CachedLinkDto(
            link.Id,
            link.UserId,
            host ?? defaultHost,
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

    private static string BuildShortUrl(string host, string slug)
    {
        var scheme = host.StartsWith("localhost") || host.StartsWith("127.0.0.1") ? "http" : "https";
        return $"{scheme}://{host}/{slug}";
    }

    private static LinkDetailDto MapDetail(Link link, string? host, string defaultHost)
    {
        var resolvedHost = host ?? defaultHost;
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
