using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShortlink.Backend.Application.Billing;

namespace WebShortlink.Backend.Api.Controllers.User;

[ApiController]
[Authorize(Roles = Domain.Enums.AppRoles.User)]
[Route("api/user/billing")]
public sealed class BillingController : ControllerBase
{
    private readonly BillingService _billingService;

    public BillingController(BillingService billingService)
    {
        _billingService = billingService;
    }

    [HttpPost("upgrade")]
    public async Task<IActionResult> Upgrade([FromBody] UpgradeSubscriptionRequestDto request, CancellationToken cancellationToken)
        => Ok(await _billingService.UpgradeAsync(request, cancellationToken));
}
