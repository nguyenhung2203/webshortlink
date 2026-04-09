using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Application.Domains;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Links;

public sealed class RedirectService
{
    private readonly string _defaultHost;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILinkCacheService _linkCacheService;
    private readonly IAnalyticsQueue _analyticsQueue;
    private readonly IPasswordHasher<Link> _passwordHasher;

    public RedirectService(
        ApplicationDbContext dbContext,
        ILinkCacheService linkCacheService,
        IAnalyticsQueue analyticsQueue,
        IPasswordHasher<Link> passwordHasher,
        Microsoft.Extensions.Options.IOptions<WebShortlink.Backend.Infrastructure.Options.AppOptions> appOptions)
    {
        _dbContext = dbContext;
        _linkCacheService = linkCacheService;
        _analyticsQueue = analyticsQueue;
        _passwordHasher = passwordHasher;
        _defaultHost = appOptions.Value.DefaultDomain;
    }

    public async Task<PublicRedirectAccessResponseDto> ResolveAsync(string host, string slug, string? password, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var startedAt = Environment.TickCount64;
        var normalizedHost = NormalizeHost(host);
        var cached = await _linkCacheService.GetAsync(normalizedHost, slug, cancellationToken);
        var link = cached ?? await ResolveFromDatabaseAsync(normalizedHost, slug, cancellationToken);
        if (link is null)
        {
            throw new AppException(ErrorCodes.NotFound, "Shortlink khong ton tai.", StatusCodes.Status404NotFound);
        }
        if (link is null)
        {
            throw new AppException(ErrorCodes.NotFound, "Shortlink không tồn tại.", StatusCodes.Status404NotFound);
        }

        ValidateRedirectPreconditions(link);

        if (!string.IsNullOrWhiteSpace(link.PasswordHash))
        {
            var placeholder = new Link { PasswordHash = link.PasswordHash };
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppException(ErrorCodes.PasswordRequired, "Link yeu cau mat khau.", StatusCodes.Status400BadRequest);
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppException(ErrorCodes.PasswordRequired, "Link yêu cầu mật khẩu.", StatusCodes.Status400BadRequest);
            }

            var verification = _passwordHasher.VerifyHashedPassword(placeholder, link.PasswordHash, password);
            if (verification == PasswordVerificationResult.Failed)
            {
                throw new AppException(ErrorCodes.InvalidPassword, "Mat khau khong dung.");
            }
            if (verification == PasswordVerificationResult.Failed)
            {
                throw new AppException(ErrorCodes.InvalidPassword, "Mật khẩu không đúng.");
            }
        }

        var reservedTotalClicks = await ReserveClickAsync(link, cancellationToken);
        var responseTimeMs = (int)(Environment.TickCount64 - startedAt);
        var targetUrl = await ResolveTargetUrlAsync(link, reservedTotalClicks, httpContext, cancellationToken);
        
        WebShortlink.Backend.Application.Analytics.AnalyticsSourceHelper.ParseAnalyticsFromRequest(
            httpContext.Request, 
            out var referrer, 
            out var normalizedSource, 
            out var utmSource, 
            out var utmMedium, 
            out var utmCampaign);

        await _analyticsQueue.EnqueueAsync(new ClickTrackingMessage
        {
            LinkId = link.Id,
            ClickedAtUtc = DateTime.UtcNow,
            Host = normalizedHost,
            Slug = slug,
            IpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            Referrer = string.IsNullOrWhiteSpace(referrer) ? "direct" : referrer,
            UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
            ResponseTimeMs = responseTimeMs,
            EventStatus = ClickEventStatus.Redirected.ToString(),
            CountryCode = DetectCountry(httpContext),
            City = DetectCity(httpContext),
            NormalizedSource = normalizedSource,
            UtmSource = utmSource,
            UtmMedium = utmMedium,
            UtmCampaign = utmCampaign
        }, cancellationToken);

        return new PublicRedirectAccessResponseDto(targetUrl);
    }

    private async Task<CachedLinkDto?> ResolveFromDatabaseAsync(string host, string slug, CancellationToken cancellationToken)
    {
        host = NormalizeHost(host);
        var link = await _dbContext.Links
            .AsNoTracking()
            .Include(x => x.Domain)
            .FirstOrDefaultAsync(
                x => x.Slug == slug
                    && !x.IsDeleted
                    && ((host == _defaultHost && x.DomainId == null)
                        || (x.Domain != null && !x.Domain.IsDeleted && x.Domain.IsVerified && x.Domain.Host == host)),
                cancellationToken);

        if (link is null)
        {
            return null;
        }

        var dto = new CachedLinkDto(link.Id, link.UserId, link.Domain?.Host ?? _defaultHost, link.Slug, link.OriginalUrl, link.Status.ToString(), link.ExpiresAtUtc, link.ClickLimit, link.PasswordHash, link.TotalClicks);
        await _linkCacheService.SetAsync(dto, cancellationToken);
        return dto;
    }

    [Obsolete("Use ValidateRedirectPreconditions instead.")]
    private static void ValidateLinkAvailability(CachedLinkDto link)
    {
        if (!Enum.TryParse<LinkStatus>(link.Status, out var status))
        {
            throw new AppException(ErrorCodes.NotFound, "Shortlink không tồn tại.", StatusCodes.Status404NotFound);
        }

        if (status is LinkStatus.Paused or LinkStatus.DisabledByAdmin)
        {
            throw new AppException(ErrorCodes.LinkDisabled, "Link hiện không khả dụng.", StatusCodes.Status400BadRequest);
        }

        if (link.ExpiresAtUtc.HasValue && link.ExpiresAtUtc.Value <= DateTime.UtcNow)
        {
            throw new AppException(ErrorCodes.LinkExpired, "Link đã hết hạn.", StatusCodes.Status400BadRequest);
        }

        if (link.ClickLimit.HasValue && link.TotalClicks >= link.ClickLimit.Value)
        {
            throw new AppException(ErrorCodes.ClickLimitReached, "Link đã vượt giới hạn click.", StatusCodes.Status400BadRequest);
        }
    }
    private static void ValidateRedirectPreconditions(CachedLinkDto link)
    {
        if (!Enum.TryParse<LinkStatus>(link.Status, out var status))
        {
            throw new AppException(ErrorCodes.NotFound, "Shortlink khong ton tai.", StatusCodes.Status404NotFound);
        }

        if (status is LinkStatus.Paused or LinkStatus.DisabledByAdmin)
        {
            throw new AppException(ErrorCodes.LinkDisabled, "Link hien khong kha dung.", StatusCodes.Status400BadRequest);
        }

        if (link.ExpiresAtUtc.HasValue && link.ExpiresAtUtc.Value <= DateTime.UtcNow)
        {
            throw new AppException(ErrorCodes.LinkExpired, "Link da het han.", StatusCodes.Status400BadRequest);
        }
    }

    private async Task<long> ReserveClickAsync(CachedLinkDto link, CancellationToken cancellationToken)
    {
        const int maxRetries = 3;

        for (var attempt = 0; attempt < maxRetries; attempt++)
        {
            var trackedLink = await _dbContext.Links
                .Include(x => x.Domain)
                .FirstOrDefaultAsync(x => x.Id == link.Id && !x.IsDeleted, cancellationToken);

            if (trackedLink is null)
            {
                throw new AppException(ErrorCodes.NotFound, "Shortlink khong ton tai.", StatusCodes.Status404NotFound);
            }

            if (trackedLink.Status is LinkStatus.Paused or LinkStatus.DisabledByAdmin)
            {
                throw new AppException(ErrorCodes.LinkDisabled, "Link hien khong kha dung.", StatusCodes.Status400BadRequest);
            }

            if (trackedLink.ExpiresAtUtc.HasValue && trackedLink.ExpiresAtUtc.Value <= DateTime.UtcNow)
            {
                throw new AppException(ErrorCodes.LinkExpired, "Link da het han.", StatusCodes.Status400BadRequest);
            }

            if (trackedLink.ClickLimit.HasValue && trackedLink.TotalClicks >= trackedLink.ClickLimit.Value)
            {
                throw new AppException(ErrorCodes.ClickLimitReached, "Link da vuot gioi han click.", StatusCodes.Status400BadRequest);
            }

            trackedLink.TotalClicks += 1;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);

                await _linkCacheService.SetAsync(
                    new CachedLinkDto(
                        trackedLink.Id,
                        trackedLink.UserId,
                        trackedLink.Domain?.Host ?? _defaultHost,
                        trackedLink.Slug,
                        trackedLink.OriginalUrl,
                        trackedLink.Status.ToString(),
                        trackedLink.ExpiresAtUtc,
                        trackedLink.ClickLimit,
                        trackedLink.PasswordHash,
                        trackedLink.TotalClicks),
                    cancellationToken);

                return trackedLink.TotalClicks;
            }
            catch (DbUpdateConcurrencyException) when (attempt < maxRetries - 1)
            {
                _dbContext.ChangeTracker.Clear();
            }
        }

        throw new AppException(ErrorCodes.Conflict, "Khong the ghi nhan click luc nay. Vui long thu lai.", StatusCodes.Status409Conflict);
    }

    private async Task<string> ResolveTargetUrlAsync(CachedLinkDto link, long reservedTotalClicks, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var rules = await _dbContext.LinkRules
            .AsNoTracking()
            .Where(x => x.LinkId == link.Id && x.IsActive)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        if (rules.Count == 0)
        {
            return link.OriginalUrl;
        }

        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        var device = DetectDevice(userAgent);
        var browser = DetectBrowser(userAgent);
        var operatingSystem = DetectOperatingSystem(userAgent);
        var country = DetectCountry(httpContext);

        foreach (var rule in rules.Where(x => x.RuleType != LinkRuleType.Rotation))
        {
            if (MatchesRule(rule, country, device, browser, operatingSystem))
            {
                return rule.TargetUrl;
            }
        }

        var percentageRules = rules.Where(x => x.RuleType == LinkRuleType.Percentage).ToList();
        if (percentageRules.Count > 0)
        {
            var totalWeight = percentageRules.Sum(x => int.TryParse(x.RuleValue, out var val) ? Math.Max(0, val) : 0);
            if (totalWeight > 0)
            {
                var rand = Random.Shared.Next(0, totalWeight);
                var current = 0;
                foreach (var rule in percentageRules)
                {
                    var weight = int.TryParse(rule.RuleValue, out var val) ? Math.Max(0, val) : 0;
                    if (weight <= 0) continue;
                    
                    current += weight;
                    if (rand < current)
                    {
                        return rule.TargetUrl;
                    }
                }
            }
        }

        var rotationRules = rules.Where(x => x.RuleType == LinkRuleType.Rotation).ToList();
        if (rotationRules.Count > 0)
        {
            var index = (int)((reservedTotalClicks - 1) % rotationRules.Count);
            return rotationRules[index].TargetUrl;
        }

        return link.OriginalUrl;
    }

    private string NormalizeHost(string host)
    {
        var normalized = string.IsNullOrWhiteSpace(host) ? _defaultHost : host.Trim().ToLowerInvariant();
        if (normalized.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || normalized.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            normalized = DomainService.NormalizeHost(normalized);
        }

        if (normalized.Contains(':'))
        {
            normalized = normalized.Split(':')[0];
        }

        if (normalized is "localhost" or "127.0.0.1" or "::1" or "default")
        {
            return _defaultHost;
        }

        return normalized;
    }

    private static bool MatchesRule(LinkRule rule, string country, string device, string browser, string operatingSystem)
    {
        var values = rule.RuleValue
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.ToLowerInvariant())
            .ToHashSet();

        return rule.RuleType switch
        {
            LinkRuleType.GeoTargeting => values.Contains(country.ToLowerInvariant()),
            LinkRuleType.DeviceTargeting => values.Contains(device.ToLowerInvariant()),
            LinkRuleType.BrowserTargeting => values.Contains(browser.ToLowerInvariant()),
            LinkRuleType.OsTargeting => values.Contains(operatingSystem.ToLowerInvariant()),
            LinkRuleType.DeepLink => values.Contains(device.ToLowerInvariant()) || values.Contains(operatingSystem.ToLowerInvariant()) || values.Contains("mobile"),
            _ => false
        };
    }

    private static string? DetectCountry(HttpContext httpContext)
    {
        var cfCountry = httpContext.Request.Headers["CF-IPCountry"].ToString();
        if (!string.IsNullOrWhiteSpace(cfCountry))
        {
            return cfCountry.ToUpperInvariant();
        }

        var testCountry = httpContext.Request.Headers["X-Test-Country"].ToString();
        if (!string.IsNullOrWhiteSpace(testCountry))
        {
            return testCountry.ToUpperInvariant();
        }

        return null;
    }

    private static string? DetectCity(HttpContext httpContext)
    {
        var cfCity = httpContext.Request.Headers["CF-IPCity"].ToString();
        if (!string.IsNullOrWhiteSpace(cfCity))
        {
            return cfCity.Trim();
        }

        var testCity = httpContext.Request.Headers["X-Test-City"].ToString();
        if (!string.IsNullOrWhiteSpace(testCity))
        {
            return testCity.Trim();
        }

        return null;
    }

    private static string DetectDevice(string userAgent)
    {
        return userAgent.Contains("android", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("iphone", StringComparison.OrdinalIgnoreCase)
            ? "Mobile"
            : "Desktop";
    }

    private static string DetectBrowser(string userAgent)
    {
        if (userAgent.Contains("edg", StringComparison.OrdinalIgnoreCase)) return "Edge";
        if (userAgent.Contains("chrome", StringComparison.OrdinalIgnoreCase)) return "Chrome";
        if (userAgent.Contains("safari", StringComparison.OrdinalIgnoreCase)) return "Safari";
        if (userAgent.Contains("firefox", StringComparison.OrdinalIgnoreCase)) return "Firefox";
        return "Other";
    }

    private static string DetectOperatingSystem(string userAgent)
    {
        if (userAgent.Contains("windows", StringComparison.OrdinalIgnoreCase)) return "Windows";
        if (userAgent.Contains("android", StringComparison.OrdinalIgnoreCase)) return "Android";
        if (userAgent.Contains("iphone", StringComparison.OrdinalIgnoreCase)) return "iOS";
        if (userAgent.Contains("mac", StringComparison.OrdinalIgnoreCase)) return "MacOS";
        return "Other";
    }
}
