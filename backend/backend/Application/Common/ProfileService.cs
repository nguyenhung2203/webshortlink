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
        return new CurrentSessionDto(
            user.Id,
            user.Email!,
            user.FullName,
            current.IsAdmin ? Domain.Enums.AppRoles.Admin : Domain.Enums.AppRoles.User,
            user.CurrentPlanId,
            user.AccountStatus.ToString());
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
        var plans = await _dbContext.Plans.AsNoTracking()
            .Include(x => x.Features)
            .Where(x => x.IsActive)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var labels = await _dbContext.SystemSettings
            .AsNoTracking()
            .Where(x => x.SettingKey.StartsWith("FeatureLabel:"))
            .ToDictionaryAsync(
                x => x.SettingKey.Substring("FeatureLabel:".Length),
                x => x.SettingValue,
                cancellationToken);

        var planDtos = new List<PlanDto>();
        foreach (var plan in plans)
        {
            var featureInfos = new List<PlanFeatureInfo>();
            foreach (var feature in plan.Features.OrderBy(x => x.Id))
            {
                if (!feature.IsEnabled)
                {
                    continue;
                }

                var labelValue = labels.GetValueOrDefault(feature.FeatureKey);
                var title = feature.FeatureKey;
                string? hint = null;

                if (!string.IsNullOrEmpty(labelValue))
                {
                    var parts = labelValue.Split('|');
                    title = parts.Length > 0 ? parts[0] : feature.FeatureKey;
                    hint = parts.Length > 1 ? parts[1] : null;
                    if (string.IsNullOrWhiteSpace(hint))
                    {
                        hint = null;
                    }

                    if (feature.LimitValue.HasValue)
                    {
                        var limit = feature.LimitValue.Value.ToString("N0");
                        title = title.Replace("{limit}", limit);
                        hint = hint?.Replace("{limit}", limit);
                    }

                    if (!string.IsNullOrEmpty(feature.FeatureValue))
                    {
                        title = title.Replace("{value}", feature.FeatureValue);
                        hint = hint?.Replace("{value}", feature.FeatureValue);
                    }
                }
                else
                {
                    title = feature.FeatureKey switch
                    {
                        "links.max_count" => $"Tạo tối đa {(feature.LimitValue.HasValue ? feature.LimitValue.Value.ToString("N0") : "Không giới hạn")} links",
                        "domains.custom" => feature.LimitValue.HasValue ? $"Gắn tối đa {feature.LimitValue} tên miền riêng" : "Hỗ trợ tên miền riêng",
                        "domains.max_count" => $"Tối đa {feature.LimitValue} tên miền riêng",
                        "analytics.advanced" => "Báo cáo analytics chi tiết",
                        "links.custom_slug" => "Tùy chỉnh hậu tố (slug)",
                        "analytics.retention_days" => feature.LimitValue.HasValue ? $"Lưu trữ báo cáo {feature.LimitValue} ngày" : "Lưu trữ báo cáo dài hạn",
                        "links.password_protection" => "Bảo vệ link bằng mật khẩu",
                        "links.expiration" => "Cài đặt ngày hết hạn",
                        "links.click_limit" => "Cài đặt giới hạn click",
                        "rules.targeting_advanced" => "Định tuyến theo thiết bị/quốc gia",
                        "reports.export" => "Xuất dữ liệu analytics (CSV)",
                        "links.social_preview" => "Tùy chỉnh thẻ xem trước MXH (OpenGraph)",
                        "links.wrapper" => "Bọc link chuyên nghiệp",
                        "links.wrapper_landing" => "Giao diện Landing Page",
                        "links.wrapper_cta" => "Khối kêu gọi hành động (CTA)",
                        _ => feature.FeatureKey
                    };

                    if (feature.FeatureKey == "analytics.advanced")
                    {
                        hint = "Nguồn, quốc gia, thiết bị";
                    }
                    else if (feature.FeatureKey == "rules.targeting_advanced")
                    {
                        hint = "A/B Testing, Geo-Targeting";
                    }
                }

                featureInfos.Add(new PlanFeatureInfo(title, hint, feature.IsEnabled));
            }

            planDtos.Add(new PlanDto(plan.Id, plan.Code, plan.Name, plan.MonthlyPrice, plan.IsActive, featureInfos));
        }

        return planDtos;
    }

    public async Task<SubscriptionDto> GetCurrentSubscriptionAsync(CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var user = await _dbContext.Users.AsNoTracking().FirstAsync(x => x.Id == current.UserId, cancellationToken);

        var subscription = await _dbContext.Subscriptions
            .AsNoTracking()
            .Include(x => x.Plan)
            .OrderByDescending(x => x.StartAtUtc)
            .FirstOrDefaultAsync(
                x => x.UserId == current.UserId
                    && x.PlanId == user.CurrentPlanId
                    && x.Status == Domain.Enums.SubscriptionStatus.Active,
                cancellationToken);

        if (subscription is null)
        {
            var plan = await _dbContext.Plans.AsNoTracking()
                .FirstAsync(x => x.Id == user.CurrentPlanId, cancellationToken);

            return new SubscriptionDto(
                Guid.Empty,
                plan.Id,
                plan.Name,
                Domain.Enums.SubscriptionStatus.Active.ToString(),
                user.CreatedAtUtc,
                user.CreatedAtUtc.AddMonths(1));
        }

        return new SubscriptionDto(
            subscription.Id,
            subscription.PlanId,
            subscription.Plan.Name,
            subscription.Status.ToString(),
            subscription.StartAtUtc,
            subscription.EndAtUtc);
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
