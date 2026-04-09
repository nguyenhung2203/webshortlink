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
            
            if (response is OgLinkDataDto og)
            {
                var html = $"""
                <!DOCTYPE html>
                <html lang="vi">
                <head>
                    <meta charset="UTF-8">
                    <title>{System.Net.WebUtility.HtmlEncode(og.OgTitle ?? "WeShort Link")}</title>
                    <meta property="og:title" content="{System.Net.WebUtility.HtmlEncode(og.OgTitle ?? "")}">
                    <meta property="og:description" content="{System.Net.WebUtility.HtmlEncode(og.OgDescription ?? "")}">
                    <meta property="og:image" content="{System.Net.WebUtility.HtmlEncode(og.OgImageUrl ?? "")}">
                    <meta property="og:url" content="https://{og.Host}/{og.Slug}">
                    <meta property="og:type" content="website">
                    <meta name="twitter:card" content="summary_large_image">
                    
                    <!-- Redirect for normal users if they somehow stay on the page -->
                    <meta http-equiv="refresh" content="0;url={System.Net.WebUtility.HtmlEncode(og.OriginalUrl)}">
                </head>
                <body>
                    <p>Đang chuyển hướng đến <a href="{System.Net.WebUtility.HtmlEncode(og.OriginalUrl)}">{System.Net.WebUtility.HtmlEncode(og.OriginalUrl)}</a>...</p>
                    <script>window.location.replace("{og.OriginalUrl}");</script>
                </body>
                </html>
                """;
                return Content(html, "text/html");
            }
            
            var redirectResponse = (PublicRedirectAccessResponseDto)response;
            return Redirect(redirectResponse.RedirectUrl);
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
