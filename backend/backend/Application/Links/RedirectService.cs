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
    private const string DefaultHost = "sho.rt";
    private readonly ApplicationDbContext _dbContext;
    private readonly ILinkCacheService _linkCacheService;
    private readonly IAnalyticsQueue _analyticsQueue;
    private readonly IPasswordHasher<Link> _passwordHasher;

    public RedirectService(
        ApplicationDbContext dbContext,
        ILinkCacheService linkCacheService,
        IAnalyticsQueue analyticsQueue,
        IPasswordHasher<Link> passwordHasher)
    {
        _dbContext = dbContext;
        _linkCacheService = linkCacheService;
        _analyticsQueue = analyticsQueue;
        _passwordHasher = passwordHasher;
    }

    public async Task<PublicRedirectAccessResponseDto> ResolveAsync(string host, string slug, string? password, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var startedAt = Environment.TickCount64;
        var normalizedHost = NormalizeHost(host);
        var cached = await _linkCacheService.GetAsync(normalizedHost, slug, cancellationToken);
        var link = cached ?? await ResolveFromDatabaseAsync(normalizedHost, slug, cancellationToken);
        if (link is null)
        {
            throw new AppException(ErrorCodes.NotFound, "Shortlink không tồn tại.", StatusCodes.Status404NotFound);
        }

        ValidateLinkAvailability(link);

        if (!string.IsNullOrWhiteSpace(link.PasswordHash))
        {
            var placeholder = new Link { PasswordHash = link.PasswordHash };
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppException(ErrorCodes.PasswordRequired, "Link yêu cầu mật khẩu.", StatusCodes.Status400BadRequest);
            }

            var verification = _passwordHasher.VerifyHashedPassword(placeholder, link.PasswordHash, password);
            if (verification == PasswordVerificationResult.Failed)
            {
                throw new AppException(ErrorCodes.InvalidPassword, "Mật khẩu không đúng.");
            }
        }

        var responseTimeMs = (int)(Environment.TickCount64 - startedAt);
        var targetUrl = await ResolveTargetUrlAsync(link, httpContext, cancellationToken);

        await _analyticsQueue.EnqueueAsync(new ClickTrackingMessage
        {
            LinkId = link.Id,
            ClickedAtUtc = DateTime.UtcNow,
            Host = normalizedHost,
            Slug = slug,
            IpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            Referrer = httpContext.Request.Headers.Referer.ToString(),
            UserAgent = httpContext.Request.Headers.UserAgent.ToString(),
            ResponseTimeMs = responseTimeMs,
            EventStatus = ClickEventStatus.Redirected.ToString()
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
                    && ((host == DefaultHost && x.DomainId == null)
                        || (x.Domain != null && !x.Domain.IsDeleted && x.Domain.IsVerified && x.Domain.Host == host)),
                cancellationToken);

        if (link is null)
        {
            return null;
        }

        var dto = new CachedLinkDto(link.Id, link.UserId, link.Domain?.Host ?? DefaultHost, link.Slug, link.OriginalUrl, link.Status.ToString(), link.ExpiresAtUtc, link.ClickLimit, link.PasswordHash, link.TotalClicks);
        await _linkCacheService.SetAsync(dto, cancellationToken);
        return dto;
    }

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
    private async Task<string> ResolveTargetUrlAsync(CachedLinkDto link, HttpContext httpContext, CancellationToken cancellationToken)
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

        var rotationRules = rules.Where(x => x.RuleType == LinkRuleType.Rotation).ToList();
        if (rotationRules.Count > 0)
        {
            var index = (int)(link.TotalClicks % rotationRules.Count);
            return rotationRules[index].TargetUrl;
        }

        return link.OriginalUrl;
    }

    private static string NormalizeHost(string host)
    {
        var normalized = string.IsNullOrWhiteSpace(host) ? DefaultHost : host.Trim().ToLowerInvariant();
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
            return DefaultHost;
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

    private static string DetectCountry(HttpContext httpContext)
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

        return "VN";
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
