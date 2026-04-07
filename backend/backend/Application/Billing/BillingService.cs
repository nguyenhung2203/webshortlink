using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Billing;

public sealed class BillingService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditLogService _auditLogService;

    public BillingService(
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IAuditLogService auditLogService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _auditLogService = auditLogService;
    }

    public async Task<UpgradeSubscriptionResponseDto> UpgradeAsync(UpgradeSubscriptionRequestDto request, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var user = await _dbContext.Users.FirstAsync(x => x.Id == current.UserId, cancellationToken);
        var targetPlan = await _dbContext.Plans.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.PlanId && x.IsActive, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Khong tim thay goi dich vu.", StatusCodes.Status404NotFound);

        if (user.CurrentPlanId == targetPlan.Id)
        {
            throw new AppException(ErrorCodes.Conflict, "Tai khoan dang o goi nay.", StatusCodes.Status409Conflict);
        }

        var activeSubscriptions = await _dbContext.Subscriptions
            .Where(x => x.UserId == current.UserId && x.Status == SubscriptionStatus.Active)
            .ToListAsync(cancellationToken);

        foreach (var activeSubscription in activeSubscriptions)
        {
            activeSubscription.Status = SubscriptionStatus.Expired;
            activeSubscription.EndAtUtc = DateTime.UtcNow;
            activeSubscription.UpdatedAtUtc = DateTime.UtcNow;
            activeSubscription.UpdatedByUserId = current.UserId.ToString();
        }

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = current.UserId,
            PlanId = targetPlan.Id,
            Status = SubscriptionStatus.Active,
            StartAtUtc = DateTime.UtcNow,
            EndAtUtc = DateTime.UtcNow.AddMonths(1),
            AutoRenew = true,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = current.UserId.ToString()
        };

        _dbContext.Subscriptions.Add(subscription);
        user.CurrentPlanId = targetPlan.Id;

        if (targetPlan.MonthlyPrice > 0)
        {
            _dbContext.Payments.Add(new Payment
            {
                Id = Guid.NewGuid(),
                SubscriptionId = subscription.Id,
                Amount = targetPlan.MonthlyPrice,
                Currency = "VND",
                Provider = "internal-demo",
                ProviderReference = $"upgrade-{subscription.Id:N}",
                Status = PaymentStatus.Paid,
                PaidAtUtc = DateTime.UtcNow,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = current.UserId.ToString()
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _auditLogService.WriteAsync(
            AuditActorType.User,
            "USR-API-BILLING-UPGRADE",
            nameof(Subscription),
            subscription.Id.ToString(),
            current.UserId,
            current.IpAddress,
            new { request.PlanId, targetPlan.Code, targetPlan.MonthlyPrice },
            cancellationToken);

        return new UpgradeSubscriptionResponseDto(
            subscription.Id,
            targetPlan.Id,
            targetPlan.Code,
            targetPlan.Name,
            targetPlan.MonthlyPrice,
            "VND",
            subscription.StartAtUtc,
            subscription.EndAtUtc,
            targetPlan.MonthlyPrice > 0 ? "Nang cap goi thanh cong." : "Chuyen goi thanh cong.");
    }
}
