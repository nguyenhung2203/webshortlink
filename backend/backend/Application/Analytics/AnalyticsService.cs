using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Analytics;

public sealed class AnalyticsService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public AnalyticsService(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<AnalyticsOverviewDto> GetOverviewAsync(CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var links = await _dbContext.Links
            .AsNoTracking()
            .Where(x => x.UserId == current.UserId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var linkIds = links.Select(x => x.Id).ToList();
        var dailyStats = await _dbContext.LinkDailyStats
            .AsNoTracking()
            .Where(x => linkIds.Contains(x.LinkId))
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
        var link = await _dbContext.Links.AsNoTracking().FirstOrDefaultAsync(x => x.Id == linkId && x.UserId == current.UserId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy link.", StatusCodes.Status404NotFound);

        var events = await _dbContext.ClickEvents.AsNoTracking().Where(x => x.LinkId == linkId).ToListAsync(cancellationToken);
        var trends = await _dbContext.LinkDailyStats.AsNoTracking().Where(x => x.LinkId == linkId).OrderBy(x => x.StatDate).ToListAsync(cancellationToken);

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
}
