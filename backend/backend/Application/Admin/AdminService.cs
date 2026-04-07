using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Analytics;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Admin;

public sealed class AdminService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditLogService _auditLogService;
    private readonly ILinkCacheService _linkCacheService;
    private readonly UserManager<AppUser> _userManager;

    public AdminService(
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IAuditLogService auditLogService,
        ILinkCacheService linkCacheService,
        UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _auditLogService = auditLogService;
        _linkCacheService = linkCacheService;
        _userManager = userManager;
    }

    public async Task<CurrentSessionDto> GetAdminSessionAsync(CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var user = await _dbContext.Users.AsNoTracking().FirstAsync(x => x.Id == current.UserId, cancellationToken);
        return new CurrentSessionDto(user.Id, user.Email!, user.FullName, AppRoles.Admin, user.CurrentPlanId, user.AccountStatus.ToString());
    }

    public async Task<AdminOverviewDto> GetOverviewAsync(CancellationToken cancellationToken)
    {
        var totalUsers = await _dbContext.Users.LongCountAsync(cancellationToken);
        var paidUsers = await _dbContext.Users.LongCountAsync(x => x.CurrentPlanId != 1, cancellationToken);
        var totalLinks = await _dbContext.Links.LongCountAsync(x => !x.IsDeleted, cancellationToken);
        var activeLinks = await _dbContext.Links.LongCountAsync(x => !x.IsDeleted && x.Status == LinkStatus.Active, cancellationToken);
        var totalClicks = await _dbContext.ClickEvents.LongCountAsync(cancellationToken);
        var uniqueClicks = await _dbContext.LinkDailyStats.SumAsync(x => (long?)x.UniqueClicks, cancellationToken) ?? 0L;
        var botClicks = await _dbContext.ClickEvents.LongCountAsync(x => x.IsBot, cancellationToken);
        var suspiciousClicks = await _dbContext.ClickEvents.LongCountAsync(x => x.IsSuspicious, cancellationToken);
        var monthlyRevenue = await _dbContext.Payments.Where(x => x.Status == PaymentStatus.Paid).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;

        return new AdminOverviewDto(
            new BusinessOverviewDto(totalUsers, paidUsers, totalUsers == 0 ? 0 : Math.Round((decimal)paidUsers / totalUsers * 100, 2), monthlyRevenue),
            new ProductOverviewDto(totalLinks, activeLinks, totalClicks, uniqueClicks),
            new OperationsOverviewDto(botClicks, suspiciousClicks, 0.2m, 2, 45));
    }

    public async Task<IReadOnlyCollection<AdminUserListItemDto>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var roleMap = await GetUserRolesMapAsync(cancellationToken);
        var users = await _dbContext.Users.AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new AdminUserListItemDto(
                x.Id,
                x.Email!,
                x.FullName,
                string.Empty,
                x.AccountStatus.ToString(),
                _dbContext.Plans.Where(p => p.Id == x.CurrentPlanId).Select(p => p.Name).FirstOrDefault() ?? "Unknown",
                x.Links.LongCount(l => !l.IsDeleted),
                x.Links.Where(l => !l.IsDeleted).Sum(l => (long?)l.TotalClicks) ?? 0,
                x.CreatedAtUtc,
                x.LastLoginAtUtc))
            .ToListAsync(cancellationToken);

        return users.Select(x => x with { Role = roleMap.GetValueOrDefault(x.Id, AppRoles.User) }).ToList();
    }

    public async Task<AdminUserDetailDto> GetUserDetailAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(x => x.Links)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy user.", StatusCodes.Status404NotFound);
        var planName = await _dbContext.Plans.Where(x => x.Id == user.CurrentPlanId).Select(x => x.Name).FirstAsync(cancellationToken);
        var roleMap = await GetUserRolesMapAsync(cancellationToken);

        return new AdminUserDetailDto(
            user.Id,
            user.Email!,
            user.FullName,
            roleMap.GetValueOrDefault(user.Id, AppRoles.User),
            user.AccountStatus.ToString(),
            user.CurrentPlanId,
            planName,
            user.CreatedAtUtc,
            user.LastLoginAtUtc,
            user.Links.Where(x => !x.IsDeleted).Select(x => new AdminUserLinkDto(x.Id, $"https://sho.rt/{x.Slug}", x.Status.ToString(), x.TotalClicks)).ToList());
    }

    public async Task<MessageResponseDto> ChangeUserStatusAsync(Guid userId, ChangeUserStatusRequestDto request, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy user.", StatusCodes.Status404NotFound);

        user.AccountStatus = request.Status.Trim().ToLowerInvariant() switch
        {
            "active" => UserAccountStatus.Active,
            "locked" => UserAccountStatus.Locked,
            "disabled" => UserAccountStatus.Disabled,
            _ => throw new AppException(ErrorCodes.ValidationFailed, "Trang thai user khong hop le.")
        };

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLogService.WriteAsync(AuditActorType.Admin, "ADM-API-005:ChangeUserStatus", "AppUser", user.Id.ToString(), current.UserId, current.IpAddress, request, cancellationToken);
        return new MessageResponseDto("Cập nhật trạng thái user thành công.");
    }

    public async Task<MessageResponseDto> ChangeUserPlanAsync(Guid userId, ChangeUserPlanRequestDto request, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy user.", StatusCodes.Status404NotFound);

        var plan = await _dbContext.Plans.FirstOrDefaultAsync(x => x.Id == request.PlanId && x.IsActive, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy gói dịch vụ.", StatusCodes.Status404NotFound);

        user.CurrentPlanId = plan.Id;
        var activeSubscriptions = await _dbContext.Subscriptions
            .Where(x => x.UserId == user.Id && x.Status == SubscriptionStatus.Active)
            .ToListAsync(cancellationToken);

        foreach (var activeSubscription in activeSubscriptions)
        {
            activeSubscription.Status = SubscriptionStatus.Expired;
            activeSubscription.EndAtUtc = DateTime.UtcNow;
            activeSubscription.UpdatedAtUtc = DateTime.UtcNow;
        }

        _dbContext.Subscriptions.Add(new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            PlanId = plan.Id,
            Status = SubscriptionStatus.Active,
            StartAtUtc = DateTime.UtcNow,
            EndAtUtc = DateTime.UtcNow.AddMonths(1),
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = current.UserId.ToString()
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLogService.WriteAsync(AuditActorType.Admin, "ADM-API-006:ChangeUserPlan", "Subscription", user.Id.ToString(), current.UserId, current.IpAddress, request, cancellationToken);
        return new MessageResponseDto("Đã cập nhật gói cho user.");
    }

    public async Task<IReadOnlyCollection<AdminLinkListItemDto>> GetLinksAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Links.AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Domain)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Where(x => !x.IsDeleted)
            .Select(x => new AdminLinkListItemDto(
                x.Id,
                $"https://sho.rt/{x.Slug}",
                x.Slug,
                x.OriginalUrl,
                x.Status.ToString(),
                x.User.Email!,
                x.TotalClicks,
                x.ClickEvents.LongCount(y => y.IsBot),
                x.AbuseReports.OrderByDescending(y => y.RiskScore).Select(y => (int?)y.RiskScore).FirstOrDefault(),
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminLinkDetailDto> GetLinkDetailAsync(Guid linkId, CancellationToken cancellationToken)
    {
        var link = await _dbContext.Links
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.ClickEvents)
            .FirstOrDefaultAsync(x => x.Id == linkId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy link.", StatusCodes.Status404NotFound);

        return new AdminLinkDetailDto(
            link.Id,
            $"https://sho.rt/{link.Slug}",
            link.Slug,
            link.OriginalUrl,
            link.Status.ToString(),
            link.User.Email!,
            link.TotalClicks,
            link.UniqueClicks,
            link.ClickEvents.LongCount(x => x.IsBot),
            link.ClickEvents.GroupBy(x => x.CountryCode ?? "Unknown")
                .OrderByDescending(x => x.Count())
                .Take(5)
                .Select(x => new KeyValueMetricDto(x.Key, x.LongCount()))
                .ToList());
    }

    public async Task<MessageResponseDto> ToggleLinkAsync(Guid linkId, bool enable, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var link = await _dbContext.Links.Include(x => x.Domain).FirstOrDefaultAsync(x => x.Id == linkId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy link.", StatusCodes.Status404NotFound);

        link.Status = enable ? LinkStatus.Active : LinkStatus.DisabledByAdmin;
        link.UpdatedAtUtc = DateTime.UtcNow;
        link.UpdatedByUserId = current.UserId.ToString();
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _linkCacheService.RemoveAsync("sho.rt", link.Slug, cancellationToken);

        await _auditLogService.WriteAsync(AuditActorType.Admin, "ADM-API-009:ToggleLink", "Link", linkId.ToString(), current.UserId, current.IpAddress, new { enable }, cancellationToken);
        return new MessageResponseDto("Cập nhật trạng thái link thành công.");
    }

    public async Task<object> GetReportsBasicAsync(CancellationToken cancellationToken)
    {
        var topPlans = await _dbContext.Users.AsNoTracking()
            .GroupBy(x => x.CurrentPlanId)
            .Select(x => new { PlanId = x.Key, Users = x.LongCount() })
            .ToListAsync(cancellationToken);

        return new { topPlans };
    }

    public async Task<object> GetSecurityOpsBasicAsync(CancellationToken cancellationToken)
    {
        var latestAudit = await _dbContext.AuditLogs.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc).Take(10).ToListAsync(cancellationToken);
        return new
        {
            botClicks = await _dbContext.ClickEvents.LongCountAsync(x => x.IsBot, cancellationToken),
            suspiciousClicks = await _dbContext.ClickEvents.LongCountAsync(x => x.IsSuspicious, cancellationToken),
            auditLogs = latestAudit.Select(x => new { x.Action, x.ResourceType, x.ResourceId, x.CreatedAtUtc })
        };
    }

    public async Task<IReadOnlyCollection<AdminAuditLogItemDto>> GetAuditLogsAsync(CancellationToken cancellationToken)
    {
        var logs = await _dbContext.AuditLogs.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc).Take(50).ToListAsync(cancellationToken);
        return logs.Select(x => new AdminAuditLogItemDto(x.Id, x.Action, x.ResourceType, x.ResourceId, x.ActorType.ToString(), x.CreatedAtUtc)).ToList();
    }

    private async Task<Dictionary<Guid, string>> GetUserRolesMapAsync(CancellationToken cancellationToken)
    {
        return await (
                from userRole in _dbContext.UserRoles
                join role in _dbContext.Roles on userRole.RoleId equals role.Id
                select new { userRole.UserId, role.Name })
            .GroupBy(x => x.UserId)
            .ToDictionaryAsync(x => x.Key, x => x.Select(y => y.Name!).FirstOrDefault() ?? AppRoles.User, cancellationToken);
    }
}
