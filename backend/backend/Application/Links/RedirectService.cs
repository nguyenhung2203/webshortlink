using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Application.Domains;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;
using WebShortlink.Backend.Infrastructure.Options;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Links;

public sealed class RedirectService
{
    private readonly string _defaultHost;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILinkCacheService _linkCacheService;
    private readonly IAnalyticsQueue _analyticsQueue;
    private readonly IPasswordHasher<Link> _passwordHasher;
    private readonly string _wrapperSigningKey;

    public RedirectService(
        ApplicationDbContext dbContext,
        ILinkCacheService linkCacheService,
        IAnalyticsQueue analyticsQueue,
        IPasswordHasher<Link> passwordHasher,
        Microsoft.Extensions.Options.IOptions<AppOptions> appOptions,
        Microsoft.Extensions.Options.IOptions<JwtOptions> jwtOptions)
    {
        _dbContext = dbContext;
        _linkCacheService = linkCacheService;
        _analyticsQueue = analyticsQueue;
        _passwordHasher = passwordHasher;
        _defaultHost = appOptions.Value.DefaultDomain;
        _wrapperSigningKey = jwtOptions.Value.SigningKey;
    }

    public async Task<object> ResolveAsync(string host, string slug, string? password, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var startedAt = Environment.TickCount64;
        var requestUserAgent = httpContext.Request.Headers.UserAgent.ToString();
        var isSocialScraper = IsSocialScraper(requestUserAgent);
        var normalizedHost = NormalizeHost(host);
        var systemDefaultHost = await GetSystemDefaultHostAsync(cancellationToken);
        var cached = isSocialScraper
            ? null
            : await _linkCacheService.GetAsync(normalizedHost, slug, cancellationToken);
        var link = cached ?? await ResolveFromDatabaseAsync(normalizedHost, systemDefaultHost, slug, cancellationToken);
        if (link is null)
        {
            throw new AppException(ErrorCodes.NotFound, "Shortlink không tồn tại.", StatusCodes.Status404NotFound);
        }

        ValidateRedirectPreconditions(link);

        if (isSocialScraper)
        {
            return new OgLinkDataDto(
                link.OriginalUrl,
                link.OgTitle ?? link.WrapperTitle,
                link.OgDescription ?? link.WrapperDescription,
                link.OgImageUrl ?? link.WrapperImageUrl,
                normalizedHost,
                slug);
        }

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

        var primaryEventStatus = link.IsWrapperEnabled
            ? ClickEventStatus.WrapperView.ToString()
            : ClickEventStatus.Redirected.ToString();

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
            EventStatus = primaryEventStatus,
            CountryCode = DetectCountry(httpContext),
            City = DetectCity(httpContext),
            NormalizedSource = normalizedSource,
            UtmSource = utmSource,
            UtmMedium = utmMedium,
            UtmCampaign = utmCampaign
        }, cancellationToken);

        return new PublicRedirectAccessResponseDto(targetUrl);
    }

    public async Task<PublicWrapperRenderDto?> BuildWrapperRenderAsync(string host, string slug, string targetUrl, CancellationToken cancellationToken)
    {
        var normalizedHost = NormalizeHost(host);
        var systemDefaultHost = await GetSystemDefaultHostAsync(cancellationToken);
        var link = await _linkCacheService.GetAsync(normalizedHost, slug, cancellationToken)
            ?? await ResolveFromDatabaseAsync(normalizedHost, systemDefaultHost, slug, cancellationToken);

        if (link is null || !link.IsWrapperEnabled)
        {
            return null;
        }

        var mode = ParseRedirectMode(link.RedirectMode);
        var encodedTarget = EncodeTarget(targetUrl);
        var signature = SignWrapperTarget(normalizedHost, slug, targetUrl);

        return new PublicWrapperRenderDto(
            $"/w/{slug}?target={encodedTarget}&sig={signature}",
            $"/c/{slug}?target={encodedTarget}&sig={signature}",
            normalizedHost,
            slug,
            targetUrl,
            mode.ToString(),
            Math.Clamp(link.DelaySeconds ?? (mode == LinkRedirectMode.Instant ? 1 : 3), 1, 30),
            link.WrapperTitle ?? link.OgTitle ?? "Bạn sắp đến trang đích",
            link.WrapperDescription ?? link.OgDescription ?? "Nhấn tiếp tục để mở liên kết.",
            link.WrapperImageUrl ?? link.OgImageUrl,
            string.IsNullOrWhiteSpace(link.ContinueButtonText) ? "Tiếp tục đến trang đích" : link.ContinueButtonText,
            string.IsNullOrWhiteSpace(link.WarningText) ? "Bạn sắp rời khỏi website hiện tại." : link.WarningText,
            string.IsNullOrWhiteSpace(link.WrapperTheme) ? "brand" : link.WrapperTheme,
            link.BrandName,
            link.BrandLogoUrl,
            link.CtaTitle,
            link.CtaDescription,
            link.CtaButtonText,
            link.CtaButtonUrl);
    }

    public bool TryReadWrapperTarget(string host, string slug, string? encodedTarget, string? signature, out string targetUrl)
    {
        targetUrl = string.Empty;
        if (string.IsNullOrWhiteSpace(encodedTarget) || string.IsNullOrWhiteSpace(signature))
        {
            return false;
        }

        try
        {
            var rawBytes = Convert.FromBase64String(PadBase64(encodedTarget.Replace('-', '+').Replace('_', '/')));
            var decoded = Encoding.UTF8.GetString(rawBytes);
            if (!Uri.TryCreate(decoded, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
            {
                return false;
            }

            var normalizedHost = NormalizeHost(host);
            var expected = SignWrapperTarget(normalizedHost, slug, decoded);
            if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(signature)))
            {
                return false;
            }

            targetUrl = decoded;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> TrackContinueAsync(string host, string slug, string targetUrl, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var normalizedHost = NormalizeHost(host);
        var systemDefaultHost = await GetSystemDefaultHostAsync(cancellationToken);
        var link = await _linkCacheService.GetAsync(normalizedHost, slug, cancellationToken)
            ?? await ResolveFromDatabaseAsync(normalizedHost, systemDefaultHost, slug, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Shortlink không tồn tại.", StatusCodes.Status404NotFound);

        ValidateRedirectPreconditions(link);

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
            ResponseTimeMs = 0,
            EventStatus = ClickEventStatus.ContinueClicked.ToString(),
            CountryCode = DetectCountry(httpContext),
            City = DetectCity(httpContext),
            NormalizedSource = normalizedSource,
            UtmSource = utmSource,
            UtmMedium = utmMedium,
            UtmCampaign = utmCampaign
        }, cancellationToken);

        return targetUrl;
    }

    private async Task<CachedLinkDto?> ResolveFromDatabaseAsync(string host, string defaultHost, string slug, CancellationToken cancellationToken)
    {
        host = NormalizeHost(host);
        defaultHost = NormalizeHost(defaultHost);
        var link = await _dbContext.Links
            .AsNoTracking()
            .Include(x => x.Domain)
            .FirstOrDefaultAsync(
                x => x.Slug == slug
                    && !x.IsDeleted
                    && (((host == _defaultHost || host == defaultHost) && x.DomainId == null)
                        || (x.Domain != null && !x.Domain.IsDeleted && x.Domain.IsVerified && x.Domain.Host == host)),
                cancellationToken);

        if (link is null)
        {
            return null;
        }

        var dto = new CachedLinkDto(
            link.Id,
            link.UserId,
            link.Domain?.Host ?? defaultHost,
            link.Slug,
            link.OriginalUrl,
            link.Status.ToString(),
            link.ExpiresAtUtc,
            link.ClickLimit,
            link.PasswordHash,
            link.TotalClicks,
            link.OgTitle,
            link.OgDescription,
            link.OgImageUrl,
            link.IsWrapperEnabled,
            link.RedirectMode.ToString(),
            link.DelaySeconds,
            link.WrapperTitle,
            link.WrapperDescription,
            link.WrapperImageUrl,
            link.ContinueButtonText,
            link.WarningText,
            link.WrapperTheme,
            link.BrandName,
            link.BrandLogoUrl,
            link.CtaTitle,
            link.CtaDescription,
            link.CtaButtonText,
            link.CtaButtonUrl);
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
                throw new AppException(ErrorCodes.NotFound, "Shortlink không tồn tại.", StatusCodes.Status404NotFound);
            }

            if (trackedLink.Status is LinkStatus.Paused or LinkStatus.DisabledByAdmin)
            {
                throw new AppException(ErrorCodes.LinkDisabled, "Link hiện không khả dụng.", StatusCodes.Status400BadRequest);
            }

            if (trackedLink.ExpiresAtUtc.HasValue && trackedLink.ExpiresAtUtc.Value <= DateTime.UtcNow)
            {
                throw new AppException(ErrorCodes.LinkExpired, "Link đã hết hạn.", StatusCodes.Status400BadRequest);
            }

            if (trackedLink.ClickLimit.HasValue && trackedLink.TotalClicks >= trackedLink.ClickLimit.Value)
            {
                throw new AppException(ErrorCodes.ClickLimitReached, "Link đã vượt giới hạn click.", StatusCodes.Status400BadRequest);
            }

            trackedLink.TotalClicks += 1;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);

                var systemDefaultHost = await GetSystemDefaultHostAsync(cancellationToken);
                await _linkCacheService.SetAsync(
                    new CachedLinkDto(
                        trackedLink.Id,
                        trackedLink.UserId,
                        trackedLink.Domain?.Host ?? systemDefaultHost,
                        trackedLink.Slug,
                        trackedLink.OriginalUrl,
                        trackedLink.Status.ToString(),
                        trackedLink.ExpiresAtUtc,
                        trackedLink.ClickLimit,
                        trackedLink.PasswordHash,
                        trackedLink.TotalClicks,
                        trackedLink.OgTitle,
                        trackedLink.OgDescription,
                        trackedLink.OgImageUrl,
                        trackedLink.IsWrapperEnabled,
                        trackedLink.RedirectMode.ToString(),
                        trackedLink.DelaySeconds,
                        trackedLink.WrapperTitle,
                        trackedLink.WrapperDescription,
                        trackedLink.WrapperImageUrl,
                        trackedLink.ContinueButtonText,
                        trackedLink.WarningText,
                        trackedLink.WrapperTheme,
                        trackedLink.BrandName,
                        trackedLink.BrandLogoUrl,
                        trackedLink.CtaTitle,
                        trackedLink.CtaDescription,
                        trackedLink.CtaButtonText,
                        trackedLink.CtaButtonUrl),
                    cancellationToken);

                return trackedLink.TotalClicks;
            }
            catch (DbUpdateConcurrencyException) when (attempt < maxRetries - 1)
            {
                _dbContext.ChangeTracker.Clear();
            }
        }

        throw new AppException(ErrorCodes.Conflict, "Không thể ghi nhận click lúc này. Vui lòng thử lại.", StatusCodes.Status409Conflict);
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
        var country = DetectCountry(httpContext) ?? "UN";

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
        if (string.IsNullOrWhiteSpace(host)) return _defaultHost;

        var normalized = host.Trim().ToLowerInvariant();
        
        // Strip protocol/port/path if present
        if (normalized.StartsWith("http://") || normalized.StartsWith("https://"))
        {
            normalized = DomainService.NormalizeHost(normalized);
        }
        
        if (normalized.Contains(':'))
        {
            normalized = normalized.Split(':')[0];
        }

        // Standardize www.
        if (normalized.StartsWith("www."))
        {
            normalized = normalized[4..];
        }

        // Treat local/technical hosts as the default domain for global links
        if (normalized is "localhost" or "127.0.0.1" or "::1" or "default" || normalized.EndsWith(".atempurl.com"))
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

    private static bool IsSocialScraper(string userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return false;

        var ua = userAgent.ToLowerInvariant();
        return ua.Contains("facebookexternalhit") || 
               ua.Contains("facebookcatalog") ||
               ua.Contains("metaexternalhit") ||
               ua.Contains("twitterbot") || 
               ua.Contains("telegrambot") || 
               ua.Contains("linkedinbot") || 
               ua.Contains("viber") || 
               ua.Contains("skypeuripreview") ||
               ua.Contains("zalo") ||
               ua.Contains("zalopc") ||
               ua.Contains("slackbot") ||
               ua.Contains("whatsapp") ||
               ua.Contains("discordbot") ||
               ua.Contains("pinterest") ||
               ua.Contains("googlebot");
    }

    private async Task<string> GetSystemDefaultHostAsync(CancellationToken cancellationToken)
    {
        var setting = await _dbContext.SystemSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SettingKey == "DefaultDomain", cancellationToken);

        return setting?.SettingValue ?? _defaultHost;
    }

    private LinkRedirectMode ParseRedirectMode(string? value)
    {
        return Enum.TryParse<LinkRedirectMode>(value, true, out var mode) ? mode : LinkRedirectMode.Instant;
    }

    private string EncodeTarget(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private string SignWrapperTarget(string host, string slug, string targetUrl)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_wrapperSigningKey));
        var payload = Encoding.UTF8.GetBytes($"{host}|{slug}|{targetUrl}");
        return Convert.ToBase64String(hmac.ComputeHash(payload)).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string PadBase64(string value)
    {
        var remainder = value.Length % 4;
        return remainder == 0 ? value : value + new string('=', 4 - remainder);
    }
}
