using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Common;

public sealed class ProfileService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<AppUser> _userManager;

    public ProfileService(ApplicationDbContext dbContext, ICurrentUserService currentUserService, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<CurrentSessionDto> GetAsync(CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var user = await _dbContext.Users.AsNoTracking().FirstAsync(x => x.Id == current.UserId, cancellationToken);
        return new CurrentSessionDto(user.Id, user.Email!, user.FullName, current.IsAdmin ? Domain.Enums.AppRoles.Admin : Domain.Enums.AppRoles.User, user.CurrentPlanId, user.AccountStatus.ToString());
    }

    public async Task<CurrentSessionDto> UpdateAsync(UpdateProfileRequestDto request, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var user = await _dbContext.Users.FirstAsync(x => x.Id == current.UserId, cancellationToken);
        user.FullName = request.FullName.Trim();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return await GetAsync(cancellationToken);
    }

    public async Task<MessageResponseDto> ChangePasswordAsync(ChangePasswordRequestDto request)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Xác nhận mật khẩu không khớp.");
        }

        var current = _currentUserService.GetRequired();
        var user = await _userManager.FindByIdAsync(current.UserId.ToString())
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy người dùng.", StatusCodes.Status404NotFound);
        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            throw new AppException(ErrorCodes.ValidationFailed, string.Join("; ", result.Errors.Select(x => x.Description)));
        }

        return new MessageResponseDto("Đổi mật khẩu thành công.");
    }

    public async Task<IReadOnlyCollection<PlanDto>> GetPlansAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Plans.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Id)
            .Select(x => new PlanDto(x.Id, x.Code, x.Name, x.MonthlyPrice, x.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<SubscriptionDto> GetCurrentSubscriptionAsync(CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var subscription = await _dbContext.Subscriptions
            .AsNoTracking()
            .Include(x => x.Plan)
            .OrderByDescending(x => x.StartAtUtc)
            .FirstOrDefaultAsync(x => x.UserId == current.UserId && x.Status == Domain.Enums.SubscriptionStatus.Active, cancellationToken);

        if (subscription is null)
        {
            var user = await _dbContext.Users.AsNoTracking()
                .FirstAsync(x => x.Id == current.UserId, cancellationToken);
            var plan = await _dbContext.Plans.AsNoTracking()
                .FirstAsync(x => x.Id == user.CurrentPlanId, cancellationToken);

            return new SubscriptionDto(Guid.Empty, plan.Id, plan.Name, Domain.Enums.SubscriptionStatus.Active.ToString(), user.CreatedAtUtc, user.CreatedAtUtc.AddMonths(1));
        }

        return new SubscriptionDto(subscription.Id, subscription.PlanId, subscription.Plan.Name, subscription.Status.ToString(), subscription.StartAtUtc, subscription.EndAtUtc);
    }

    public async Task<IReadOnlyCollection<PaymentHistoryItemDto>> GetPaymentHistoryAsync(CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        return await _dbContext.Payments.AsNoTracking()
            .Where(x => x.Subscription.UserId == current.UserId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new PaymentHistoryItemDto(x.Id, x.Amount, x.Currency, x.Status.ToString(), x.PaidAtUtc))
            .ToListAsync(cancellationToken);
    }
}
