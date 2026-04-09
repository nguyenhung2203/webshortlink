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
    private readonly Microsoft.Extensions.Options.IOptions<WebShortlink.Backend.Infrastructure.Options.AppOptions> _appOptions;

    public RedirectController(RedirectService redirectService, Microsoft.Extensions.Options.IOptions<WebShortlink.Backend.Infrastructure.Options.AppOptions> appOptions)
    {
        _redirectService = redirectService;
        _appOptions = appOptions;
    }

    [HttpGet("/api/public/resolve/{slug}")]
    public async Task<IActionResult> Resolve(string slug, CancellationToken cancellationToken)
    {
        var response = await _redirectService.ResolveAsync(ResolveHost(), slug, null, HttpContext, cancellationToken);
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
        try
        {
            var response = await _redirectService.ResolveAsync(ResolveHost(), slug, null, HttpContext, cancellationToken);
            return Redirect(response.RedirectUrl);
        }
        catch (AppException ex)
        {
            var feBaseUrl = _appOptions.Value.FrontendUrl.TrimEnd('/');
            return ex.ErrorCode switch
            {
                ErrorCodes.NotFound      => Redirect($"{feBaseUrl}/link-not-found"),
                ErrorCodes.LinkExpired   => Redirect($"{feBaseUrl}/link-expired"),
                ErrorCodes.LinkDisabled  => Redirect($"{feBaseUrl}/link-disabled"),
                ErrorCodes.ClickLimitReached => Redirect($"{feBaseUrl}/link-limit-reached"),
                ErrorCodes.PasswordRequired  => Redirect($"{feBaseUrl}/link/{slug}/unlock"),
                _ => Redirect($"{feBaseUrl}/link-not-found"),
            };
        }
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
