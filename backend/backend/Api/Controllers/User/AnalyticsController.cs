using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShortlink.Backend.Application.Analytics;

namespace WebShortlink.Backend.Api.Controllers.User;

[ApiController]
[Authorize(Roles = Domain.Enums.AppRoles.User)]
[Route("api/user/analytics")]
public sealed class AnalyticsController : ControllerBase
{
    private readonly AnalyticsService _analyticsService;

    public AnalyticsController(AnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
        => Ok(await _analyticsService.GetOverviewAsync(cancellationToken));

    [HttpGet("links/{linkId:guid}")]
    public async Task<IActionResult> GetLinkAnalytics(Guid linkId, CancellationToken cancellationToken)
        => Ok(await _analyticsService.GetLinkAnalyticsAsync(linkId, cancellationToken));

    /// <summary>FIX-019: Export link analytics as CSV (Pro/Plus only)</summary>
    [HttpGet("links/{linkId:guid}/export-csv")]
    public async Task<IActionResult> ExportCsv(Guid linkId, CancellationToken cancellationToken)
    {
        var (content, fileName) = await _analyticsService.ExportLinkAnalyticsCsvAsync(linkId, cancellationToken);
        return File(content, "text/csv; charset=utf-8", fileName);
    }
}

