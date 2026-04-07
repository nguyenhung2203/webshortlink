using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Application.Links;

namespace WebShortlink.Backend.Api.Controllers.Public;

[ApiController]
[AllowAnonymous]
public sealed class RedirectController : ControllerBase
{
    private readonly RedirectService _redirectService;

    public RedirectController(RedirectService redirectService)
    {
        _redirectService = redirectService;
    }

    [HttpGet("/api/public/resolve/{slug}")]
    public async Task<IActionResult> Resolve(string slug, CancellationToken cancellationToken)
    {
        var response = await _redirectService.ResolveAsync(ResolveHost(), slug, Request.Query["password"], HttpContext, cancellationToken);
        return Ok(response);
    }

    [HttpPost("/api/public/resolve/{slug}/password")]
    public async Task<IActionResult> SubmitPassword(string slug, [FromBody] PublicRedirectAccessRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _redirectService.ResolveAsync(ResolveHost(), slug, request.Password, HttpContext, cancellationToken);
        return Ok(response);
    }

    [HttpGet("/{slug}")]
    public async Task<IActionResult> RedirectBySlug(string slug, CancellationToken cancellationToken)
    {
        var response = await _redirectService.ResolveAsync(ResolveHost(), slug, Request.Query["password"], HttpContext, cancellationToken);
        return Redirect(response.RedirectUrl);
    }

    private string ResolveHost()
    {
        var forwarded = Request.Headers["X-Shortlink-Host"].ToString();
        if (!string.IsNullOrWhiteSpace(forwarded))
        {
            return forwarded;
        }

        return Request.Host.Host;
    }
}
