using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Analytics;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Application.Domains;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;
using WebShortlink.Backend.Infrastructure.Options;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Admin;

public sealed class AdminService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditLogService _auditLogService;
    private readonly IAnalyticsQueue _analyticsQueue;
    private readonly ILinkCacheService _linkCacheService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IDomainVerificationService _domainVerificationService;
    private readonly string _defaultHost;

    public AdminService(
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IAuditLogService auditLogService,
        IAnalyticsQueue analyticsQueue,
        ILinkCacheService linkCacheService,
        UserManager<AppUser> userManager,
        IOptions<AppOptions> appOptions,
        IDomainVerificationService domainVerificationService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _auditLogService = auditLogService;
        _analyticsQueue = analyticsQueue;
        _linkCacheService = linkCacheService;
        _userManager = userManager;
        _domainVerificationService = domainVerificationService;
        _defaultHost = appOptions.Value.DefaultDomain;
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
        var monthlyRevenue = await _dbContext.Subscriptions
            .Where(x => x.Status == SubscriptionStatus.Active)
            .SumAsync(x => (decimal?)x.Plan.MonthlyPrice, cancellationToken) ?? 0m;
        var queueLag = await _analyticsQueue.GetPendingCountAsync(cancellationToken);
        var errorRate = await CalculateRedirectErrorRateAsync(cancellationToken);
        var redirectLatencyP95 = await CalculateRedirectLatencyP95Async(cancellationToken);

        return new AdminOverviewDto(
            new BusinessOverviewDto(totalUsers, paidUsers, totalUsers == 0 ? 0 : Math.Round((decimal)paidUsers / totalUsers * 100, 2), monthlyRevenue),
            new ProductOverviewDto(totalLinks, activeLinks, totalClicks, uniqueClicks),
            new OperationsOverviewDto(botClicks, suspiciousClicks, errorRate, (int)Math.Min(queueLag, int.MaxValue), redirectLatencyP95));
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
            user.Links.Where(x => !x.IsDeleted).Select(x => new AdminUserLinkDto(x.Id, BuildShortUrl(_defaultHost, x.Slug), x.Status.ToString(), x.TotalClicks)).ToList());
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
            _ => throw new AppException(ErrorCodes.ValidationFailed, "Trạng thái user không hợp lệ.")
        };

        // Prevent locking/disabling ADMIN accounts
        if (user.AccountStatus != UserAccountStatus.Active)
        {
            var isAdmin = await _userManager.IsInRoleAsync(user, AppRoles.Admin);
            if (isAdmin)
            {
                throw new AppException(ErrorCodes.Forbidden, "Không thể khóa hoặc vô hiệu hóa tài khoản Quản trị viên.", StatusCodes.Status403Forbidden);
            }
        }

        if (user.AccountStatus is UserAccountStatus.Locked or UserAccountStatus.Disabled)
        {
            var openSessions = await _dbContext.RefreshSessions
                .Where(x => x.UserId == user.Id && x.RevokedAtUtc == null)
                .ToListAsync(cancellationToken);

            foreach (var session in openSessions)
            {
                session.RevokedAtUtc = DateTime.UtcNow;
                session.RevokedReason = $"Admin changed account status to {user.AccountStatus}";
                session.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

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
        var rawLinks = await _dbContext.Links.AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Domain)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Where(x => !x.IsDeleted)
            .Select(x => new {
                x.Id,
                x.Slug,
                x.OriginalUrl,
                x.Status,
                UserEmail = x.User.Email,
                x.TotalClicks,
                BotClicks = x.ClickEvents.LongCount(y => y.IsBot),
                HighestRiskScore = x.AbuseReports.OrderByDescending(y => y.RiskScore).Select(y => (int?)y.RiskScore).FirstOrDefault(),
                x.CreatedAtUtc,
                DomainHost = x.Domain != null ? x.Domain.Host : null
            })
            .ToListAsync(cancellationToken);

        return rawLinks.Select(x => new AdminLinkListItemDto(
                x.Id,
                BuildShortUrl(x.DomainHost ?? _defaultHost, x.Slug),
                x.Slug,
                x.OriginalUrl,
                x.Status.ToString(),
                x.UserEmail!,
                x.TotalClicks,
                x.BotClicks,
                x.HighestRiskScore,
                x.CreatedAtUtc)).ToList();
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
            BuildShortUrl(_defaultHost, link.Slug),
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
        await _linkCacheService.RemoveAsync(link.Domain?.Host ?? _defaultHost, link.Slug, cancellationToken);

        await _auditLogService.WriteAsync(AuditActorType.Admin, "ADM-API-009:ToggleLink", "Link", linkId.ToString(), current.UserId, current.IpAddress, new { enable }, cancellationToken);
        return new MessageResponseDto("Cập nhật trạng thái link thành công.");
    }

    public async Task<AdminReportsDto> GetReportsBasicAsync(CancellationToken cancellationToken)
    {
        var totalUsers = await _dbContext.Users.LongCountAsync(cancellationToken);
        var totalLinks = await _dbContext.Links.LongCountAsync(x => !x.IsDeleted, cancellationToken);
        var totalClicks = await _dbContext.ClickEvents.LongCountAsync(cancellationToken);
        var activeSubscriptions = await _dbContext.Subscriptions.LongCountAsync(x => x.Status == SubscriptionStatus.Active, cancellationToken);

        var topPlans = await _dbContext.Users.AsNoTracking()
            .GroupBy(x => x.CurrentPlanId)
            .Select(x => new { PlanId = x.Key, Count = x.LongCount() })
            .ToListAsync(cancellationToken);

        var planNames = await _dbContext.Plans.AsNoTracking()
            .ToDictionaryAsync(x => x.Id, x => x.Name, cancellationToken);

        var breakdown = topPlans
            .Select(x => new AdminPlanBreakdownDto(
                x.PlanId,
                planNames.GetValueOrDefault(x.PlanId, "Unknown"),
                x.Count,
                totalUsers == 0 ? 0 : Math.Round((decimal)x.Count / totalUsers * 100, 2)))
            .OrderByDescending(x => x.Count)
            .ToList();

        return new AdminReportsDto(totalUsers, totalLinks, totalClicks, activeSubscriptions, breakdown);
    }

    public async Task<AdminSecurityDto> GetSecurityOpsBasicAsync(CancellationToken cancellationToken)
    {
        var sinceUtc = DateTime.UtcNow.Date;
        var failedLoginsToday = await _dbContext.AuditLogs.LongCountAsync(
            x => x.Action == "PUB-API-002:LoginFailed" && x.CreatedAtUtc >= sinceUtc,
            cancellationToken);
        var suspiciousClicks = await _dbContext.ClickEvents.LongCountAsync(x => x.IsSuspicious, cancellationToken);
        var lockedAccounts = await _dbContext.Users.LongCountAsync(x => x.AccountStatus == UserAccountStatus.Locked, cancellationToken);
        var queueLag = await _analyticsQueue.GetPendingCountAsync(cancellationToken);
        var errorRate = await CalculateRedirectErrorRateAsync(cancellationToken);
        var redirectLatencyP95 = await CalculateRedirectLatencyP95Async(cancellationToken);

        var healthItems = new List<AdminSystemHealthItemDto>
        {
            new("Redis Analytics Queue", queueLag > 30 ? "WARN" : "OK", $"{queueLag} pending"),
            new("Redirect Error Rate", errorRate > 5 ? "WARN" : "OK", $"{errorRate:0.##}%"),
            new("Redirect Latency P95", redirectLatencyP95 > 250 ? "WARN" : "OK", $"{redirectLatencyP95} ms"),
            new("Locked Accounts", lockedAccounts > 0 ? "WARN" : "OK", lockedAccounts.ToString())
        };

        return new AdminSecurityDto(failedLoginsToday, suspiciousClicks, lockedAccounts, healthItems);
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

    private async Task<decimal> CalculateRedirectErrorRateAsync(CancellationToken cancellationToken)
    {
        var totalEvents = await _dbContext.ClickEvents.LongCountAsync(cancellationToken);
        if (totalEvents == 0)
        {
            return 0;
        }

        var failedEvents = await _dbContext.ClickEvents.LongCountAsync(x => x.EventStatus != ClickEventStatus.Redirected, cancellationToken);
        return Math.Round((decimal)failedEvents / totalEvents * 100, 2);
    }

    private async Task<int> CalculateRedirectLatencyP95Async(CancellationToken cancellationToken)
    {
        var latencies = await _dbContext.ClickEvents.AsNoTracking()
            .OrderByDescending(x => x.ClickedAtUtc)
            .Select(x => x.ResponseTimeMs)
            .Take(500)
            .ToListAsync(cancellationToken);

        if (latencies.Count == 0)
        {
            return 0;
        }

        latencies.Sort();
        var index = (int)Math.Ceiling(latencies.Count * 0.95) - 1;
        index = Math.Max(0, Math.Min(index, latencies.Count - 1));
        return latencies[index];
    }

    public async Task<IReadOnlyCollection<AdminPaymentListItemDto>> GetPaymentsAsync(CancellationToken cancellationToken)
    {
        var payments = await _dbContext.Payments.AsNoTracking()
            .Include(x => x.Subscription)
                .ThenInclude(x => x.Plan)
            .Include(x => x.Subscription.User)
            .OrderBy(x => x.Status == PaymentStatus.Pending ? 0 : 1)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Select(x => new AdminPaymentListItemDto(
                x.Id,
                x.Subscription.User.Email!,
                x.Subscription.Plan.Name,
                x.Amount,
                x.Status.ToString(),
                x.Provider,
                x.CreatedAtUtc,
                x.PaidAtUtc))
            .ToListAsync(cancellationToken);

        return payments;
    }

    public async Task<MessageResponseDto> ApprovePaymentAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        
        var payment = await _dbContext.Payments
            .Include(x => x.Subscription)
                .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy giao dịch", StatusCodes.Status404NotFound);

        if (payment.Status != PaymentStatus.Pending)
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Giao dịch này không ở trạng thái chờ duyệt.", StatusCodes.Status400BadRequest);
        }

        payment.Status = PaymentStatus.Paid;
        payment.PaidAtUtc = DateTime.UtcNow;
        payment.UpdatedAtUtc = DateTime.UtcNow;
        payment.UpdatedByUserId = current.UserId.ToString();

        var subscription = payment.Subscription;
        subscription.Status = SubscriptionStatus.Active;
        subscription.StartAtUtc = DateTime.UtcNow;
        subscription.EndAtUtc = DateTime.UtcNow.AddMonths(1);
        subscription.UpdatedAtUtc = DateTime.UtcNow;
        subscription.UpdatedByUserId = current.UserId.ToString();

        var user = subscription.User;
        user.CurrentPlanId = subscription.PlanId;

        var activeSubscriptions = await _dbContext.Subscriptions
            .Where(x => x.UserId == user.Id && x.Status == SubscriptionStatus.Active && x.Id != subscription.Id)
            .ToListAsync(cancellationToken);

        foreach (var activeSub in activeSubscriptions)
        {
            activeSub.Status = SubscriptionStatus.Expired;
            activeSub.EndAtUtc = DateTime.UtcNow;
            activeSub.UpdatedAtUtc = DateTime.UtcNow;
            activeSub.UpdatedByUserId = current.UserId.ToString();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLogService.WriteAsync(AuditActorType.Admin, "ADM-API-BILLING-APPROVE", "Payment", payment.Id.ToString(), current.UserId, current.IpAddress, new { paymentId, user.Email, user.CurrentPlanId }, cancellationToken);

        return new MessageResponseDto("Đã duyệt thanh toán và kích hoạt gói thành công.");
    }

    public async Task<IReadOnlyCollection<AdminDomainListItemDto>> GetDomainsAsync(CancellationToken cancellationToken)
    {
        var defaultDomainHost = await _dbContext.SystemSettings
            .Where(x => x.SettingKey == "DefaultDomain")
            .Select(x => x.SettingValue)
            .FirstOrDefaultAsync(cancellationToken);
            
        var defaultHost = defaultDomainHost ?? string.Empty;
        var nullDomainLinksCount = await _dbContext.Links.LongCountAsync(x => x.DomainId == null && !x.IsDeleted, cancellationToken);

        return await _dbContext.Domains.AsNoTracking()
            .Include(x => x.User)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Where(x => !x.IsDeleted)
            .Select(x => new AdminDomainListItemDto(
                x.Id,
                x.Host,
                x.IsVerified,
                x.AdminFeedback,
                x.VerifiedAtUtc,
                x.CreatedAtUtc,
                x.User.Email!,
                (x.Host == defaultHost ? nullDomainLinksCount : 0) + x.Links.LongCount(l => !l.IsDeleted),
                x.IsGlobal,
                x.ExpiredAtUtc,
                x.UserNotes,
                x.Host == defaultHost))
            .ToListAsync(cancellationToken);
    }

    public async Task<MessageResponseDto> SetDefaultDomainAsync(Guid domainId, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var domain = await _dbContext.Domains.FirstOrDefaultAsync(x => x.Id == domainId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy domain", StatusCodes.Status404NotFound);

        if (!domain.IsGlobal || !domain.IsVerified)
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Chỉ có thể đặt Default Domain cho các domain hệ thống (GLOBAL) và đã được Verified.", StatusCodes.Status400BadRequest);
        }

        var setting = await _dbContext.SystemSettings.FirstOrDefaultAsync(x => x.SettingKey == "DefaultDomain", cancellationToken);
        if (setting == null)
        {
            setting = new SystemSetting
            {
                Id = Guid.NewGuid(),
                SettingKey = "DefaultDomain",
                SettingValue = domain.Host,
                GroupName = "Domains",
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = current.UserId.ToString()
            };
            _dbContext.SystemSettings.Add(setting);
        }
        else
        {
            setting.SettingValue = domain.Host;
            setting.UpdatedAtUtc = DateTime.UtcNow;
            setting.UpdatedByUserId = current.UserId.ToString();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLogService.WriteAsync(AuditActorType.Admin, "ADM-API-DOMAIN-SET-DEFAULT", "SystemSetting", setting.Id.ToString(), current.UserId, current.IpAddress, new { domain.Host }, cancellationToken);

        return new MessageResponseDto("Đã đặt tên miền này thành mặc định (System Default Domain).");
    }

    public async Task<MessageResponseDto> VerifyDomainAsync(Guid domainId, AdminVerifyDomainRequestDto request, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var domain = await _dbContext.Domains.FirstOrDefaultAsync(x => x.Id == domainId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy domain", StatusCodes.Status404NotFound);

        domain.IsVerified = true;
        domain.AdminFeedback = request.AdminFeedback;
        domain.VerifiedAtUtc = DateTime.UtcNow;
        domain.UpdatedAtUtc = DateTime.UtcNow;
        domain.UpdatedByUserId = current.UserId.ToString();

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLogService.WriteAsync(AuditActorType.Admin, "ADM-API-DOMAIN-VERIFY", "Domain", domain.Id.ToString(), current.UserId, current.IpAddress, new { domain.Host, request.AdminFeedback }, cancellationToken);

        return new MessageResponseDto("Đã chuyển trạng thái domain thành Verified và lưu phản hồi.");
    }

    public async Task<MessageResponseDto> DeleteDomainAsync(Guid domainId, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var domain = await _dbContext.Domains.FirstOrDefaultAsync(x => x.Id == domainId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy domain", StatusCodes.Status404NotFound);

        var isDefaultDomain = await _dbContext.SystemSettings
            .AsNoTracking()
            .AnyAsync(x => x.SettingKey == "DefaultDomain" && x.SettingValue == domain.Host, cancellationToken);
        if (isDefaultDomain)
        {
            throw new AppException(ErrorCodes.Conflict, "Không thể xóa domain đang được đặt làm mặc định hệ thống.", StatusCodes.Status409Conflict);
        }

        var linkedLinks = await _dbContext.Links.AnyAsync(x => x.DomainId == domainId && !x.IsDeleted, cancellationToken);
        if (linkedLinks)
        {
            throw new AppException(ErrorCodes.Conflict, "Domain đang được sử dụng bởi shortlink.", StatusCodes.Status409Conflict);
        }

        domain.IsDeleted = true;
        domain.DeletedAtUtc = DateTime.UtcNow;
        domain.DeletedByUserId = current.UserId.ToString();

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLogService.WriteAsync(AuditActorType.Admin, "ADM-API-DOMAIN-DELETE", "Domain", domain.Id.ToString(), current.UserId, current.IpAddress, new { domain.Host }, cancellationToken);

        return new MessageResponseDto("Đã xóa domain.");
    }

    /// <summary>Admin: Gọi DNS check thật sự (HTTP/.well-known) để kiểm tra domain đã trỏ đúng chưa.</summary>
    public async Task<AdminDomainDnsCheckResultDto> CheckDomainDnsAsync(Guid domainId, CancellationToken cancellationToken)
    {
        var domain = await _dbContext.Domains.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == domainId && !x.IsDeleted, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy domain", StatusCodes.Status404NotFound);

        var verified = await _domainVerificationService.VerifyDomainOwnershipAsync(domain.Host, domain.VerificationToken, cancellationToken);

        if (verified && !domain.IsVerified)
        {
            // Tự động cập nhật trạng thái nếu DNS đã đúng
            var tracked = await _dbContext.Domains.FirstAsync(x => x.Id == domainId, cancellationToken);
            tracked.IsVerified = true;
            tracked.VerifiedAtUtc = DateTime.UtcNow;
            tracked.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new AdminDomainDnsCheckResultDto(
            verified,
            domain.Host,
            verified
                ? "DNS hợp lệ. File /.well-known/webshortlink-verification.txt trả về đúng token."
                : "DNS chưa đúng. Domain chưa trỏ về server hoặc file verification chưa tồn tại.");
    }

    /// <summary>Admin: Tạo domain thay cho một user bất kỳ (Hoặc tạo System Global Domain).</summary>
    public async Task<AdminDomainListItemDto> CreateDomainForUserAsync(AdminCreateDomainForUserRequestDto request, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();

        // Nếu là System Domain (IsGlobal) mà không gán user, mặc định gán cho admin hiện tại
        var targetUserId = request.UserId ?? current.UserId;

        var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == targetUserId, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy user.", StatusCodes.Status404NotFound);

        var normalizedHost = Domains.DomainService.NormalizeHost(request.Host);

        var exists = await _dbContext.Domains.AnyAsync(x => x.Host == normalizedHost && !x.IsDeleted, cancellationToken);
        if (exists)
            throw new AppException(ErrorCodes.Conflict, "Domain đã tồn tại trong hệ thống.", StatusCodes.Status409Conflict);

        var domain = new CustomDomain
        {
            Id = Guid.NewGuid(),
            UserId = targetUserId,
            Host = normalizedHost,
            VerificationToken = Convert.ToHexString(Guid.NewGuid().ToByteArray()).ToLowerInvariant(),
            IsVerified = false,
            IsGlobal = request.IsGlobal,
            ExpiredAtUtc = request.ExpiredAtUtc,
            UserNotes = request.UserNotes,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = current.UserId.ToString()
        };

        _dbContext.Domains.Add(domain);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.WriteAsync(AuditActorType.Admin, "ADM-API-DOMAIN-CREATE", "Domain", domain.Id.ToString(), current.UserId, current.IpAddress, new { domain.Host, targetUserId, request.IsGlobal }, cancellationToken);

        return new AdminDomainListItemDto(domain.Id, domain.Host, domain.IsVerified, domain.AdminFeedback, domain.VerifiedAtUtc, domain.CreatedAtUtc, user.Email!, 0, domain.IsGlobal, domain.ExpiredAtUtc, domain.UserNotes, false);
    }

    private string BuildShortUrl(string host, string slug)
    {
        var scheme = host.StartsWith("localhost") || host.StartsWith("127.0.0.1") ? "http" : "https";
        return $"{scheme}://{host}/{slug}";
    }

    public async Task<IReadOnlyCollection<AdminPlanDetailDto>> GetPlansWithFeaturesAsync(CancellationToken cancellationToken)
    {
        var plans = await _dbContext.Plans.AsNoTracking()
            .Include(p => p.Features)
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);

        return plans.Select(p => new AdminPlanDetailDto(
            p.Id,
            p.Code,
            p.Name,
            p.MonthlyPrice,
            p.IsActive,
            p.Features.Select(f => new AdminPlanFeatureDto(
                f.Id,
                f.FeatureKey,
                f.IsEnabled,
                f.LimitValue,
                f.FeatureValue)).ToList()
        )).ToList();
    }

    public async Task<MessageResponseDto> UpdatePlanFeatureAsync(int planId, string featureKey, AdminUpdateFeatureRequestDto request, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var plan = await _dbContext.Plans.FirstOrDefaultAsync(x => x.Id == planId, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy gói dịch vụ.", StatusCodes.Status404NotFound);

        var feature = await _dbContext.PlanFeatures
            .FirstOrDefaultAsync(x => x.PlanId == planId && x.FeatureKey == featureKey, cancellationToken);

        if (feature == null)
        {
            feature = new PlanFeature
            {
                PlanId = planId,
                FeatureKey = featureKey,
                IsEnabled = request.IsEnabled,
                LimitValue = request.LimitValue,
                FeatureValue = request.FeatureValue,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = current.UserId.ToString()
            };
            _dbContext.PlanFeatures.Add(feature);
        }
        else
        {
            feature.IsEnabled = request.IsEnabled;
            feature.LimitValue = request.LimitValue;
            feature.FeatureValue = request.FeatureValue;
            feature.UpdatedAtUtc = DateTime.UtcNow;
            feature.UpdatedByUserId = current.UserId.ToString();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLogService.WriteAsync(AuditActorType.Admin, "ADM-API-PLAN-FEATURE-UPDATE", "PlanFeature", feature.Id.ToString(), current.UserId, current.IpAddress, new { planId, featureKey, request }, cancellationToken);

        return new MessageResponseDto($"Đã cập nhật tính năng '{featureKey}' cho gói '{plan.Name}'.");
    }

    public async Task<IReadOnlyCollection<AdminFeatureLabelDto>> GetFeatureLabelsAsync(CancellationToken cancellationToken)
    {
        const string prefix = "FeatureLabel:";
        var settings = await _dbContext.SystemSettings
            .AsNoTracking()
            .Where(x => x.SettingKey.StartsWith(prefix))
            .ToListAsync(cancellationToken);

        return settings.Select(s =>
        {
            var parts = (s.SettingValue ?? "").Split('|');
            return new AdminFeatureLabelDto(
                s.SettingKey[prefix.Length..],   // strip prefix
                parts.Length > 0 ? parts[0] : s.SettingKey,
                parts.Length > 1 ? parts[1] : "",
                parts.Length > 2 ? parts[2] : "toggle");
        }).ToList();
    }

    public async Task<MessageResponseDto> SaveFeatureLabelAsync(AdminSaveFeatureLabelDto request, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        const string prefix = "FeatureLabel:";
        var key = prefix + request.FeatureKey;
        var value = $"{request.Label}|{request.Description}|{request.FeatureType}";

        var setting = await _dbContext.SystemSettings.FirstOrDefaultAsync(x => x.SettingKey == key, cancellationToken);
        if (setting == null)
        {
            setting = new SystemSetting { Id = Guid.NewGuid(), SettingKey = key, SettingValue = value, GroupName = "PlanFeatureLabels", CreatedAtUtc = DateTime.UtcNow, CreatedByUserId = current.UserId.ToString() };
            _dbContext.SystemSettings.Add(setting);
        }
        else
        {
            setting.SettingValue = value;
            setting.UpdatedAtUtc = DateTime.UtcNow;
            setting.UpdatedByUserId = current.UserId.ToString();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new MessageResponseDto($"Đã lưu nhãn cho tính năng '{request.FeatureKey}'.");
    }

    public async Task<MessageResponseDto> DeleteFeatureLabelAsync(string featureKey, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        const string prefix = "FeatureLabel:";
        var key = prefix + featureKey;

        var setting = await _dbContext.SystemSettings.FirstOrDefaultAsync(x => x.SettingKey == key, cancellationToken);
        if (setting != null)
        {
            _dbContext.SystemSettings.Remove(setting);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        // Also delete related PlanFeatures across all plans
        var planFeatures = await _dbContext.PlanFeatures.Where(x => x.FeatureKey == featureKey).ToListAsync(cancellationToken);
        if (planFeatures.Any())
        {
            _dbContext.PlanFeatures.RemoveRange(planFeatures);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        await _auditLogService.WriteAsync(AuditActorType.Admin, "ADM-API-FEATURE-DELETE", "PlanFeature", featureKey, current.UserId, current.IpAddress, new { featureKey }, cancellationToken);
        return new MessageResponseDto($"Đã xóa tính năng '{featureKey}' khỏi tất cả các gói.");
    }
}
