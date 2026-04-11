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
                var pageTitle = System.Net.WebUtility.HtmlEncode(og.OgTitle ?? "WeShort Link");
                var pageDesc = System.Net.WebUtility.HtmlEncode(og.OgDescription ?? "Tiết kiệm thời gian với đường link rút gọn chuyên nghiệp.");
                var pageImage = og.OgImageUrl != null ? System.Net.WebUtility.HtmlEncode(og.OgImageUrl) : "";
                var pageUrl = $"https://{og.Host}/{og.Slug}";
                var destUrl = System.Net.WebUtility.HtmlEncode(og.OriginalUrl);
                var safeDestJs = og.OriginalUrl.Replace("\\", "\\\\").Replace("\"", "\\\"");

                // Build og:image block only if image URL exists
                var ogImageBlock = !string.IsNullOrWhiteSpace(pageImage)
                    ? $"<meta property=\"og:image\" content=\"{pageImage}\"><meta property=\"og:image:secure_url\" content=\"{pageImage}\"><meta property=\"og:image:type\" content=\"image/jpeg\"><meta property=\"og:image:width\" content=\"1200\"><meta property=\"og:image:height\" content=\"630\"><meta name=\"twitter:image\" content=\"{pageImage}\">"
                    : "";

                var html = "<!DOCTYPE html><html lang=\"vi\"><head>" +
                    "<meta charset=\"UTF-8\">" +
                    "<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">" +
                    $"<title>{pageTitle}</title>" +
                    $"<meta name=\"description\" content=\"{pageDesc}\">" +
                    "<meta property=\"og:type\" content=\"website\">" +
                    $"<meta property=\"og:url\" content=\"{pageUrl}\">" +
                    $"<meta property=\"og:title\" content=\"{pageTitle}\">" +
                    $"<meta property=\"og:description\" content=\"{pageDesc}\">" +
                    "<meta property=\"og:site_name\" content=\"WeShort\">" +
                    ogImageBlock +
                    "<meta name=\"twitter:card\" content=\"summary_large_image\">" +
                    $"<meta name=\"twitter:title\" content=\"{pageTitle}\">" +
                    $"<meta name=\"twitter:description\" content=\"{pageDesc}\">" +
                    $"<meta http-equiv=\"refresh\" content=\"0;url={destUrl}\">" +
                    "</head><body>" +
                    $"<p>Đang chuyển hướng... <a href=\"{destUrl}\">{destUrl}</a></p>" +
                    $"<script>setTimeout(function(){{window.location.replace(\"{safeDestJs}\");}},100);</script>" +
                    "</body></html>";

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
