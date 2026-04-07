using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShortlink.Backend.Application.Common;

namespace WebShortlink.Backend.Api.Controllers.User;

[ApiController]
[Authorize(Roles = Domain.Enums.AppRoles.User)]
[Route("api/user")]
public sealed class ProfileController : ControllerBase
{
    private readonly ProfileService _profileService;

    public ProfileController(ProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        => Ok(await _profileService.GetAsync(cancellationToken));

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request, CancellationToken cancellationToken)
        => Ok(await _profileService.UpdateAsync(request, cancellationToken));

    [HttpPut("security/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        => Ok(await _profileService.ChangePasswordAsync(request));

    [HttpGet("plans")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPlans(CancellationToken cancellationToken)
        => Ok(await _profileService.GetPlansAsync(cancellationToken));

    [HttpGet("subscription")]
    public async Task<IActionResult> GetSubscription(CancellationToken cancellationToken)
        => Ok(await _profileService.GetCurrentSubscriptionAsync(cancellationToken));

    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments(CancellationToken cancellationToken)
        => Ok(await _profileService.GetPaymentHistoryAsync(cancellationToken));
}
