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
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy gói dịch vụ.", StatusCodes.Status404NotFound);

        if (user.CurrentPlanId == targetPlan.Id)
        {
            throw new AppException(ErrorCodes.Conflict, "Tài khoản đang ở gói này.", StatusCodes.Status409Conflict);
        }

        var isPaidPlan = targetPlan.MonthlyPrice > 0;

        if (isPaidPlan)
        {
            var validExistingSub = await _dbContext.Subscriptions
                .FirstOrDefaultAsync(x => x.UserId == current.UserId
                    && x.PlanId == targetPlan.Id
                    && x.Status == SubscriptionStatus.Active
                    && x.EndAtUtc > DateTime.UtcNow, cancellationToken);

            if (validExistingSub != null)
            {
                user.CurrentPlanId = targetPlan.Id;
                await _dbContext.SaveChangesAsync(cancellationToken);

                await _auditLogService.WriteAsync(
                    AuditActorType.User,
                    "USR-API-BILLING-RESTORE",
                    nameof(Subscription),
                    validExistingSub.Id.ToString(),
                    current.UserId,
                    current.IpAddress,
                    new { request.PlanId, targetPlan.Code },
                    cancellationToken);

                return new UpgradeSubscriptionResponseDto(
                    validExistingSub.Id,
                    targetPlan.Id,
                    targetPlan.Code,
                    targetPlan.Name,
                    targetPlan.MonthlyPrice,
                    "VND",
                    validExistingSub.StartAtUtc,
                    validExistingSub.EndAtUtc,
                    "Đã khôi phục gói cước bạn đã mua trước đó vì vẫn còn hạn.");
            }
        }

        var subscriptionStatus = isPaidPlan ? SubscriptionStatus.Pending : SubscriptionStatus.Active;

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = current.UserId,
            PlanId = targetPlan.Id,
            Status = subscriptionStatus,
            StartAtUtc = DateTime.UtcNow,
            EndAtUtc = DateTime.UtcNow.AddMonths(1),
            AutoRenew = true,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = current.UserId.ToString()
        };

        _dbContext.Subscriptions.Add(subscription);

        // Only explicitly change the CurrentPlanId immediately if it's a free switch.
        // For paid plans, user retains old plan until payment approved.
        if (!isPaidPlan)
        {
            user.CurrentPlanId = targetPlan.Id;
            // Removed premature expiration of active subscriptions so user can restore them later.
        }

        Payment? payment = null;
        if (isPaidPlan)
        {
            payment = new Payment
            {
                Id = Guid.NewGuid(),
                SubscriptionId = subscription.Id,
                Amount = targetPlan.MonthlyPrice,
                Currency = "VND",
                Provider = "VietQR",
                ProviderReference = null,
                Status = PaymentStatus.Pending,
                PaidAtUtc = null,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByUserId = current.UserId.ToString()
            };
            _dbContext.Payments.Add(payment);
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
            payment?.Id ?? subscription.Id,
            targetPlan.Id,
            targetPlan.Code,
            targetPlan.Name,
            targetPlan.MonthlyPrice,
            "VND",
            subscription.StartAtUtc,
            subscription.EndAtUtc,
            isPaidPlan
                ? "Đã tạo yêu cầu thanh toán. Đang chờ xác nhận."
                : "Chuyển đổi gói cước thành công.");
    }

    public async Task<PaymentHistoryDto> GetPaymentDetailAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var payment = await _dbContext.Payments
            .Include(x => x.Subscription)
                .ThenInclude(x => x.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == paymentId && x.Subscription.UserId == current.UserId, cancellationToken)
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy giao dịch", StatusCodes.Status404NotFound);

        return new PaymentHistoryDto(
            payment.Id,
            payment.Subscription.Plan.Name,
            payment.Amount,
            payment.Currency,
            payment.Status.ToString(),
            payment.CreatedAtUtc,
            payment.PaidAtUtc);
    }
}
