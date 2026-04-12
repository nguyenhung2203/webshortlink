using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Infrastructure.Workers;

public sealed class ClickAnalyticsWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ClickAnalyticsWorker> _logger;

    public ClickAnalyticsWorker(IServiceScopeFactory serviceScopeFactory, ILogger<ClickAnalyticsWorker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var queue = scope.ServiceProvider.GetRequiredService<IAnalyticsQueue>();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var linkCacheService = scope.ServiceProvider.GetRequiredService<ILinkCacheService>();

                var message = await queue.DequeueAsync(stoppingToken);
                if (message is null)
                {
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                var deviceType = DetectDevice(message.UserAgent);
                var browser = DetectBrowser(message.UserAgent);
                var os = DetectOperatingSystem(message.UserAgent);
                var isBot = message.UserAgent.Contains("bot", StringComparison.OrdinalIgnoreCase);
                var eventStatus = Enum.TryParse<ClickEventStatus>(message.EventStatus, true, out var parsedStatus)
                    ? parsedStatus
                    : ClickEventStatus.Redirected;
                var shouldAggregateClick = eventStatus is ClickEventStatus.Redirected or ClickEventStatus.WrapperView;
                var fingerprint = $"{message.LinkId}:{MaskIp(message.IpAddress)}:{browser}:{message.ClickedAtUtc:yyyy-MM-dd}";
                var uniqueFingerprint = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(fingerprint)));

                var maskedIp = MaskIp(message.IpAddress);
                var isUnique = !await dbContext.ClickEvents.AsNoTracking()
                    .AnyAsync(x => x.LinkId == message.LinkId && x.FingerprintHash == uniqueFingerprint, stoppingToken);

                dbContext.ClickEvents.Add(new ClickEvent
                {
                    Id = Guid.NewGuid(),
                    LinkId = message.LinkId,
                    ClickedAtUtc = message.ClickedAtUtc,
                    IpAddress = message.IpAddress,
                    MaskedIp = maskedIp,
                    CountryCode = message.CountryCode,
                    City = message.City,
                    DeviceType = deviceType,
                    Browser = browser,
                    OperatingSystem = os,
                    Referrer = string.IsNullOrWhiteSpace(message.Referrer) ? "direct" : message.Referrer,
                    UserAgent = message.UserAgent,
                    FingerprintHash = uniqueFingerprint,
                    EventStatus = eventStatus,
                    NormalizedSource = message.NormalizedSource,
                    UtmSource = message.UtmSource,
                    UtmMedium = message.UtmMedium,
                    UtmCampaign = message.UtmCampaign,
                    IsBot = isBot,
                    IsSuspicious = isBot,
                    ResponseTimeMs = message.ResponseTimeMs,
                    CreatedAtUtc = DateTime.UtcNow
                });

                var link = await dbContext.Links
                    .Include(x => x.Domain)
                    .FirstOrDefaultAsync(x => x.Id == message.LinkId, stoppingToken);
                if (link is null)
                {
                    _logger.LogWarning("Khong tim thay link {LinkId} de aggregate analytics.", message.LinkId);
                    await dbContext.SaveChangesAsync(stoppingToken);
                    continue;
                }

                if (shouldAggregateClick && isUnique)
                {
                    link.UniqueClicks += 1;
                }

                if (shouldAggregateClick)
                {
                    var statDate = DateOnly.FromDateTime(message.ClickedAtUtc);
                    var daily = await dbContext.LinkDailyStats.FirstOrDefaultAsync(x => x.LinkId == message.LinkId && x.StatDate == statDate, stoppingToken);
                    if (daily is null)
                    {
                        daily = new LinkDailyStat
                        {
                            Id = Guid.NewGuid(),
                            LinkId = message.LinkId,
                            StatDate = statDate,
                            CreatedAtUtc = DateTime.UtcNow
                        };
                        dbContext.LinkDailyStats.Add(daily);
                    }

                    daily.TotalClicks += 1;
                    daily.UniqueClicks += isUnique ? 1 : 0;
                    daily.BotClicks += isBot ? 1 : 0;
                    daily.SuspiciousClicks += isBot ? 1 : 0;

                    var bucket = new DateTime(message.ClickedAtUtc.Year, message.ClickedAtUtc.Month, message.ClickedAtUtc.Day, message.ClickedAtUtc.Hour, 0, 0, DateTimeKind.Utc);
                    var hourly = await dbContext.LinkHourlyStats.FirstOrDefaultAsync(x => x.LinkId == message.LinkId && x.HourBucketUtc == bucket, stoppingToken);
                    if (hourly is null)
                    {
                        hourly = new LinkHourlyStat
                        {
                            Id = Guid.NewGuid(),
                            LinkId = message.LinkId,
                            HourBucketUtc = bucket,
                            CreatedAtUtc = DateTime.UtcNow
                        };
                        dbContext.LinkHourlyStats.Add(hourly);
                    }

                    hourly.TotalClicks += 1;
                    hourly.UniqueClicks += isUnique ? 1 : 0;
                    hourly.BotClicks += isBot ? 1 : 0;
                }

                await dbContext.SaveChangesAsync(stoppingToken);
                await linkCacheService.SetAsync(
                    new Application.Links.CachedLinkDto(
                        link.Id,
                        link.UserId,
                        link.Domain?.Host ?? message.Host,
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
                        link.CtaButtonUrl),
                    stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Host is shutting down, ignore
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Analytics worker gặp lỗi khi xử lý click event.");
                if (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }

    private static string MaskIp(string ip)
    {
        var parts = ip.Split('.');
        return parts.Length == 4 ? $"{parts[0]}.{parts[1]}.xxx.xxx" : "masked";
    }

    private static string DetectDevice(string userAgent)
    {
        return userAgent.Contains("android", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("iphone", StringComparison.OrdinalIgnoreCase)
            ? "Mobile"
            : "Desktop";
    }

    private static string DetectBrowser(string userAgent)
    {
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
