using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public sealed class PlansController : ControllerBase
{
    private readonly AppDataStore _store;
    private readonly DemoTimeProvider _clock;

    public PlansController(AppDataStore store, DemoTimeProvider clock)
    {
        _store = store;
        _clock = clock;
    }

    [HttpGet("plans")]
    [AllowAnonymous]
    public IActionResult Plans()
    {
        return Ok(_store.Plans);
    }

    [HttpGet("subscription/current")]
    [Authorize]
    public IActionResult CurrentSubscription()
    {
        return Ok(_store.GetCurrentSubscription(User));
    }

    [HttpPost("subscription/upgrade")]
    [Authorize]
    public IActionResult Upgrade(UpgradePlanRequest request)
    {
        try
        {
            return Ok(_store.UpgradePlan(User, request.PlanId, _clock.UtcNow));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public sealed record UpgradePlanRequest(int PlanId);
