using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShortlink.Backend.Application.Admin;
using WebShortlink.Backend.Application.Analytics;
using WebShortlink.Backend.Application.Authentication;
using WebShortlink.Backend.Application.Billing;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Application.Links;

namespace WebShortlink.Backend.Api.Controllers.Public;

[ApiController]
public sealed class LegacyCompatibilityController : ControllerBase
{
    [HttpPost("/api/auth/register")]
    [AllowAnonymous]
    public async Task<IActionResult> LegacyRegister(
        [FromBody] LegacyRegisterRequest request,
        [FromServices] AuthService authService,
        CancellationToken cancellationToken)
    {
        var response = await authService.RegisterAsync(
            new RegisterRequestDto(request.FullName, request.Email, request.Password, request.Password, null),
            cancellationToken);

        return Ok(new
        {
            message = response.Message,
            code = response.Code,
            userId = response.UserId,
            email = response.Email
        });
    }

    [HttpPost("/api/auth/login")]
    [AllowAnonymous]
    public async Task<IActionResult> LegacyLogin(
        [FromBody] LegacyLoginRequest request,
        [FromServices] AuthService authService,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(
            new LoginRequestDto(request.Email, request.Password, null),
            cancellationToken);

        return Ok(new
        {
            token = response.AccessToken,
            refreshToken = response.RefreshToken,
            expiresAtUtc = response.ExpiresAtUtc,
            user = new
            {
                id = response.User.Id,
                fullName = response.User.FullName,
                email = response.User.Email,
                role = response.User.Role,
                status = response.User.AccountStatus,
                currentPlanId = response.User.CurrentPlanId
            }
        });
    }

    [HttpGet("/api/auth/me")]
    [Authorize]
    public async Task<IActionResult> LegacyCurrentSession(
        [FromServices] AuthService authService,
        CancellationToken cancellationToken)
        => Ok(await authService.GetCurrentSessionAsync(cancellationToken));

    [HttpGet("/api/links")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyLinks(
        [FromServices] LinkService linkService,
        CancellationToken cancellationToken)
        => Ok(await linkService.GetMineAsync(new UserLinkFilterDto(), cancellationToken));

    [HttpPost("/api/links")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyCreateLink(
        [FromBody] CreateLinkRequestDto request,
        [FromServices] LinkService linkService,
        CancellationToken cancellationToken)
        => Ok(await linkService.CreateAsync(request, cancellationToken));

    [HttpGet("/api/links/{linkId:guid}")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyGetLinkDetail(
        Guid linkId,
        [FromServices] LinkService linkService,
        CancellationToken cancellationToken)
        => Ok(await linkService.GetMyDetailAsync(linkId, cancellationToken));

    [HttpPut("/api/links/{linkId:guid}")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyUpdateLink(
        Guid linkId,
        [FromBody] UpdateLinkRequestDto request,
        [FromServices] LinkService linkService,
        CancellationToken cancellationToken)
        => Ok(await linkService.UpdateAsync(linkId, request, cancellationToken));

    [HttpPatch("/api/links/{linkId:guid}/pause")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyPauseLink(
        Guid linkId,
        [FromServices] LinkService linkService,
        CancellationToken cancellationToken)
        => Ok(await linkService.ToggleAsync(linkId, new ToggleLinkStatusRequestDto(false), cancellationToken));

    [HttpPatch("/api/links/{linkId:guid}/resume")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyResumeLink(
        Guid linkId,
        [FromServices] LinkService linkService,
        CancellationToken cancellationToken)
        => Ok(await linkService.ToggleAsync(linkId, new ToggleLinkStatusRequestDto(true), cancellationToken));

    [HttpGet("/api/analytics/overview")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyAnalyticsOverview(
        [FromServices] AnalyticsService analyticsService,
        CancellationToken cancellationToken)
    {
        var response = await analyticsService.GetOverviewAsync(cancellationToken);
        return Ok(new
        {
            metrics = new
            {
                totalClicks = response.TotalClicks,
                uniqueClicks = response.UniqueClicks,
                botClicks = response.BotClicks,
                activeLinks = response.ActiveLinks
            },
            trend = response.Trends.Select(x => new
            {
                date = x.Bucket,
                totalClicks = x.TotalClicks,
                uniqueClicks = x.UniqueClicks,
                botClicks = x.BotClicks
            }),
            topLinks = response.TopLinks.Select(x => new
            {
                id = x.LinkId,
                shortUrl = x.ShortUrl,
                totalClicks = x.TotalClicks,
                uniqueClicks = x.UniqueClicks,
                status = x.Status
            })
        });
    }

    [HttpGet("/api/analytics/links/{linkId:guid}")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyLinkAnalytics(
        Guid linkId,
        [FromServices] AnalyticsService analyticsService,
        CancellationToken cancellationToken)
    {
        var response = await analyticsService.GetLinkAnalyticsAsync(linkId, cancellationToken);
        return Ok(new
        {
            link = new { id = response.LinkId },
            metrics = new
            {
                totalClicks = response.TotalClicks,
                uniqueClicks = response.UniqueClicks,
                botClicks = response.BotClicks
            },
            sources = response.TopReferrers.Select(x => new { label = x.Label, value = x.Value }),
            devices = response.TopDevices.Select(x => new { label = x.Label, value = x.Value }),
            countries = response.TopCountries.Select(x => new { label = x.Label, value = x.Value }),
            trend = response.Trends.Select(x => new
            {
                date = x.Bucket,
                totalClicks = x.TotalClicks,
                uniqueClicks = x.UniqueClicks,
                botClicks = x.BotClicks
            })
        });
    }

    [HttpGet("/api/plans")]
    [AllowAnonymous]
    public async Task<IActionResult> LegacyPlans(
        [FromServices] ProfileService profileService,
        CancellationToken cancellationToken)
        => Ok(await profileService.GetPlansAsync(cancellationToken));

    [HttpGet("/api/subscription/current")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyCurrentSubscription(
        [FromServices] ProfileService profileService,
        CancellationToken cancellationToken)
    {
        var subscription = await profileService.GetCurrentSubscriptionAsync(cancellationToken);
        return Ok(new
        {
            id = subscription.Id,
            status = subscription.Status,
            startAtUtc = subscription.StartAtUtc,
            endAtUtc = subscription.EndAtUtc,
            plan = new
            {
                id = subscription.PlanId,
                name = subscription.PlanName
            }
        });
    }

    [HttpPost("/api/subscription/upgrade")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyUpgradeSubscription(
        [FromBody] UpgradeSubscriptionRequestDto request,
        [FromServices] BillingService billingService,
        CancellationToken cancellationToken)
        => Ok(await billingService.UpgradeAsync(request, cancellationToken));

    [HttpGet("/api/payments/history")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyPaymentHistory(
        [FromServices] ProfileService profileService,
        CancellationToken cancellationToken)
        => Ok(await profileService.GetPaymentHistoryAsync(cancellationToken));

    [HttpGet("/api/me/profile")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyProfile(
        [FromServices] ProfileService profileService,
        CancellationToken cancellationToken)
        => Ok(await profileService.GetAsync(cancellationToken));

    [HttpPut("/api/me/profile")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyUpdateProfile(
        [FromBody] UpdateProfileRequestDto request,
        [FromServices] ProfileService profileService,
        CancellationToken cancellationToken)
        => Ok(await profileService.UpdateAsync(request, cancellationToken));

    [HttpPut("/api/me/password")]
    [Authorize(Roles = Domain.Enums.AppRoles.User)]
    public async Task<IActionResult> LegacyChangePassword(
        [FromBody] LegacyChangePasswordRequest request,
        [FromServices] ProfileService profileService)
        => Ok(await profileService.ChangePasswordAsync(new ChangePasswordRequestDto(request.CurrentPassword, request.NewPassword, request.NewPassword)));

    [HttpGet("/api/admin/dashboard")]
    [Authorize(Roles = Domain.Enums.AppRoles.Admin)]
    public async Task<IActionResult> LegacyAdminDashboard(
        [FromServices] AdminService adminService,
        CancellationToken cancellationToken)
        => Ok(await adminService.GetOverviewAsync(cancellationToken));

    [HttpPatch("/api/admin/users/{userId:guid}/lock")]
    [Authorize(Roles = Domain.Enums.AppRoles.Admin)]
    public async Task<IActionResult> LegacyLockUser(
        Guid userId,
        [FromServices] AdminService adminService,
        CancellationToken cancellationToken)
        => Ok(await adminService.ChangeUserStatusAsync(userId, new ChangeUserStatusRequestDto("locked"), cancellationToken));

    [HttpPatch("/api/admin/users/{userId:guid}/unlock")]
    [Authorize(Roles = Domain.Enums.AppRoles.Admin)]
    public async Task<IActionResult> LegacyUnlockUser(
        Guid userId,
        [FromServices] AdminService adminService,
        CancellationToken cancellationToken)
        => Ok(await adminService.ChangeUserStatusAsync(userId, new ChangeUserStatusRequestDto("active"), cancellationToken));

    [HttpPatch("/api/admin/links/{linkId:guid}/block")]
    [Authorize(Roles = Domain.Enums.AppRoles.Admin)]
    public async Task<IActionResult> LegacyBlockLink(
        Guid linkId,
        [FromServices] AdminService adminService,
        CancellationToken cancellationToken)
        => Ok(await adminService.ToggleLinkAsync(linkId, false, cancellationToken));

    [HttpPatch("/api/admin/links/{linkId:guid}/unblock")]
    [Authorize(Roles = Domain.Enums.AppRoles.Admin)]
    public async Task<IActionResult> LegacyUnblockLink(
        Guid linkId,
        [FromServices] AdminService adminService,
        CancellationToken cancellationToken)
        => Ok(await adminService.ToggleLinkAsync(linkId, true, cancellationToken));

    public sealed record LegacyRegisterRequest(string FullName, string Email, string Password);
    public sealed record LegacyLoginRequest(string Email, string Password);
    public sealed record LegacyChangePasswordRequest(string CurrentPassword, string NewPassword);
}
