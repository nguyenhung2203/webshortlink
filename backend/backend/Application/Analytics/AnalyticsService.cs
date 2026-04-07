using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Analytics;

public sealed class AnalyticsService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPlanCapabilityService _planCapabilityService;

    // Default retention fallback if plan feature not configured
    private const int DefaultRetentionDays = 30;

    public AnalyticsService(ApplicationDbContext dbContext, ICurrentUserService currentUserService, IPlanCapabilityService planCapabilityService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _planCapabilityService = planCapabilityService;
    }

    public async Task<AnalyticsOverviewDto> GetOverviewAsync(CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();

        // FIX-018: Enforce retention window based on plan
        var retentionDays = await _planCapabilityService.GetLimitAsync(current.UserId, "analytics.retention_days", cancellationToken)
            ?? DefaultRetentionDays;
        var retentionCutoff = DateTime.UtcNow.AddDays(-retentionDays);

        var links = await _dbContext.Links
            .AsNoTracking()
            .Where(x => x.UserId == current.UserId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var linkIds = links.Select(x => x.Id).ToList();
        var dailyStats = await _dbContext.LinkDailyStats
            .AsNoTracking()
            .Where(x => linkIds.Contains(x.LinkId) && x.StatDate >= DateOnly.FromDateTime(retentionCutoff))
            .OrderBy(x => x.StatDate)
            .ToListAsync(cancellationToken);

        return new AnalyticsOverviewDto(
            links.Sum(x => x.TotalClicks),
            links.Sum(x => x.UniqueClicks),
            dailyStats.Sum(x => x.BotClicks),
            links.LongCount(x => x.Status == Domain.Enums.LinkStatus.Active),
            dailyStats.Select(x => new TrendPointDto(x.StatDate.ToString("yyyy-MM-dd"), x.TotalClicks, x.UniqueClicks, x.BotClicks)).ToList(),
            links.OrderByDescending(x => x.TotalClicks)
                .Take(5)
                .Select(x => new TopLinkDto(x.Id, $"https://sho.rt/{x.Slug}", x.TotalClicks, x.UniqueClicks, x.Status.ToString()))
                .ToList());
    }

    public async Task<LinkAnalyticsDto> GetLinkAnalyticsAsync(Guid linkId, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        await _planCapabilityService.EnsureFeatureEnabledAsync(current.UserId, "analytics.advanced", cancellationToken);

        var link = await _dbContext.Links.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == linkId && x.UserId == current.UserId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy link.", StatusCodes.Status404NotFound);

        // FIX-018: Enforce retention window
        var retentionDays = await _planCapabilityService.GetLimitAsync(current.UserId, "analytics.retention_days", cancellationToken)
            ?? DefaultRetentionDays;
        var retentionCutoff = DateTime.UtcNow.AddDays(-retentionDays);

        var events = await _dbContext.ClickEvents.AsNoTracking()
            .Where(x => x.LinkId == linkId && x.ClickedAtUtc >= retentionCutoff)
            .ToListAsync(cancellationToken);

        var trends = await _dbContext.LinkDailyStats.AsNoTracking()
            .Where(x => x.LinkId == linkId && x.StatDate >= DateOnly.FromDateTime(retentionCutoff))
            .OrderBy(x => x.StatDate)
            .ToListAsync(cancellationToken);

        return new LinkAnalyticsDto(
            link.Id,
            link.TotalClicks,
            link.UniqueClicks,
            events.LongCount(x => x.IsBot),
            trends.Select(x => new TrendPointDto(x.StatDate.ToString("yyyy-MM-dd"), x.TotalClicks, x.UniqueClicks, x.BotClicks)).ToList(),
            events.GroupBy(x => x.CountryCode ?? "Unknown").OrderByDescending(x => x.Count()).Take(5).Select(x => new KeyValueMetricDto(x.Key, x.LongCount())).ToList(),
            events.GroupBy(x => x.DeviceType).OrderByDescending(x => x.Count()).Take(5).Select(x => new KeyValueMetricDto(x.Key, x.LongCount())).ToList(),
            events.GroupBy(x => x.Referrer).OrderByDescending(x => x.Count()).Take(5).Select(x => new KeyValueMetricDto(x.Key, x.LongCount())).ToList());
    }

    /// <summary>
    /// FIX-019: Export link analytics as CSV for Pro/Plus plans.
    /// </summary>
    public async Task<(byte[] Content, string FileName)> ExportLinkAnalyticsCsvAsync(Guid linkId, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        await _planCapabilityService.EnsureFeatureEnabledAsync(current.UserId, "reports.export", cancellationToken);

        var link = await _dbContext.Links.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == linkId && x.UserId == current.UserId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy link.", StatusCodes.Status404NotFound);

        var retentionDays = await _planCapabilityService.GetLimitAsync(current.UserId, "analytics.retention_days", cancellationToken)
            ?? DefaultRetentionDays;
        var retentionCutoff = DateTime.UtcNow.AddDays(-retentionDays);

        var events = await _dbContext.ClickEvents.AsNoTracking()
            .Where(x => x.LinkId == linkId && x.ClickedAtUtc >= retentionCutoff)
            .OrderByDescending(x => x.ClickedAtUtc)
            .ToListAsync(cancellationToken);

        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Thời gian,Quốc gia,Thành phố,Thiết bị,Trình duyệt,Hệ điều hành,Nguồn,Bot,IP (masked)");
        foreach (var ev in events)
        {
            csv.AppendLine($"{ev.ClickedAtUtc:yyyy-MM-dd HH:mm:ss},{ev.CountryCode},{ev.City},{ev.DeviceType},{ev.Browser},{ev.OperatingSystem},{ev.Referrer},{(ev.IsBot ? "Yes" : "No")},{ev.MaskedIp}");
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        var fileName = $"analytics_{link.Slug}_{DateTime.UtcNow:yyyyMMdd}.csv";
        return (bytes, fileName);
    }
}
