using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShortlink.Backend.Application.Admin;

namespace WebShortlink.Backend.Api.Controllers.Admin;

[ApiController]
[Authorize(Roles = Domain.Enums.AppRoles.Admin)]
[Route("api/admin")]
public sealed class AdminController : ControllerBase
{
    private readonly AdminService _adminService;

    public AdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("session")]
    public async Task<IActionResult> Session(CancellationToken cancellationToken)
        => Ok(await _adminService.GetAdminSessionAsync(cancellationToken));

    [HttpGet("overview")]
    public async Task<IActionResult> Overview(CancellationToken cancellationToken)
        => Ok(await _adminService.GetOverviewAsync(cancellationToken));

    [HttpGet("users")]
    public async Task<IActionResult> Users(CancellationToken cancellationToken)
        => Ok(await _adminService.GetUsersAsync(cancellationToken));

    [HttpGet("users/{userId:guid}")]
    public async Task<IActionResult> UserDetail(Guid userId, CancellationToken cancellationToken)
        => Ok(await _adminService.GetUserDetailAsync(userId, cancellationToken));

    [HttpPatch("users/{userId:guid}/status")]
    public async Task<IActionResult> ChangeUserStatus(Guid userId, [FromBody] ChangeUserStatusRequestDto request, CancellationToken cancellationToken)
        => Ok(await _adminService.ChangeUserStatusAsync(userId, request, cancellationToken));

    [HttpPatch("users/{userId:guid}/plan")]
    public async Task<IActionResult> ChangeUserPlan(Guid userId, [FromBody] ChangeUserPlanRequestDto request, CancellationToken cancellationToken)
        => Ok(await _adminService.ChangeUserPlanAsync(userId, request, cancellationToken));

    [HttpGet("links")]
    public async Task<IActionResult> Links(CancellationToken cancellationToken)
        => Ok(await _adminService.GetLinksAsync(cancellationToken));

    [HttpGet("links/{linkId:guid}")]
    public async Task<IActionResult> LinkDetail(Guid linkId, CancellationToken cancellationToken)
        => Ok(await _adminService.GetLinkDetailAsync(linkId, cancellationToken));

    [HttpPatch("links/{linkId:guid}/enable")]
    public async Task<IActionResult> EnableLink(Guid linkId, CancellationToken cancellationToken)
        => Ok(await _adminService.ToggleLinkAsync(linkId, true, cancellationToken));

    [HttpPatch("links/{linkId:guid}/disable")]
    public async Task<IActionResult> DisableLink(Guid linkId, CancellationToken cancellationToken)
        => Ok(await _adminService.ToggleLinkAsync(linkId, false, cancellationToken));

    [HttpGet("reports/basic")]
    public async Task<IActionResult> ReportsBasic(CancellationToken cancellationToken)
        => Ok(await _adminService.GetReportsBasicAsync(cancellationToken));

    [HttpGet("reports")]
    public async Task<IActionResult> Reports(CancellationToken cancellationToken)
        => Ok(await _adminService.GetReportsBasicAsync(cancellationToken));

    [HttpGet("security/basic")]
    public async Task<IActionResult> SecurityBasic(CancellationToken cancellationToken)
        => Ok(await _adminService.GetSecurityOpsBasicAsync(cancellationToken));

    [HttpGet("security")]
    public async Task<IActionResult> Security(CancellationToken cancellationToken)
        => Ok(await _adminService.GetSecurityOpsBasicAsync(cancellationToken));

    [HttpGet("audit-logs")]
    public async Task<IActionResult> AuditLogs(CancellationToken cancellationToken)
        => Ok(await _adminService.GetAuditLogsAsync(cancellationToken));

    [HttpGet("payments")]
    public async Task<IActionResult> Payments(CancellationToken cancellationToken)
        => Ok(await _adminService.GetPaymentsAsync(cancellationToken));

    [HttpPatch("payments/{paymentId:guid}/approve")]
    public async Task<IActionResult> ApprovePayment(Guid paymentId, CancellationToken cancellationToken)
        => Ok(await _adminService.ApprovePaymentAsync(paymentId, cancellationToken));
}
