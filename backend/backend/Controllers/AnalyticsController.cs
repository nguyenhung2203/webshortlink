using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/analytics")]
public sealed class AnalyticsController : ControllerBase
{
    private readonly AppDataStore _store;

    public AnalyticsController(AppDataStore store)
    {
        _store = store;
    }

    [HttpGet("overview")]
    public IActionResult Overview()
    {
        return Ok(_store.GetAnalyticsOverview(User));
    }

    [HttpGet("links/{id:guid}")]
    public IActionResult LinkDetail(Guid id)
    {
        try
        {
            return Ok(_store.GetLinkAnalytics(User, id));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("links/{id:guid}/timeseries")]
    public IActionResult LinkTimeseries(Guid id)
    {
        try
        {
            return Ok(_store.GetLinkTimeseries(User, id));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
