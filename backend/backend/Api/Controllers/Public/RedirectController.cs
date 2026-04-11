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
        if (response is PublicRedirectAccessResponseDto redirectResponse)
        {
            var wrapper = await _redirectService.BuildWrapperRenderAsync(ResolveHost(), slug, redirectResponse.RedirectUrl, cancellationToken);
            if (wrapper is not null)
            {
                return Ok(new PublicRedirectAccessResponseDto(wrapper.WrapperUrl));
            }
        }
        return Ok(response);
    }

    [HttpGet("/w/{slug}")]
    public async Task<IActionResult> WrapperBySlug(string slug, [FromQuery] string? target, [FromQuery] string? sig, CancellationToken cancellationToken)
    {
        try
        {
            if (!_redirectService.TryReadWrapperTarget(ResolveHost(), slug, target, sig, out var targetUrl))
            {
                throw new AppException(ErrorCodes.NotFound, "Wrapper khong hop le.", StatusCodes.Status404NotFound);
            }

            var wrapper = await _redirectService.BuildWrapperRenderAsync(ResolveHost(), slug, targetUrl, cancellationToken)
                ?? throw new AppException(ErrorCodes.NotFound, "Khong tim thay wrapper.", StatusCodes.Status404NotFound);

            return Content(RenderWrapperHtml(wrapper), "text/html");
        }
        catch (AppException ex)
        {
            var feBaseUrl = _appOptions.Value.FrontendUrl.TrimEnd('/');
            return ex.ErrorCode switch
            {
                ErrorCodes.NotFound => Redirect($"{feBaseUrl}/link-not-found"),
                ErrorCodes.LinkExpired => Redirect($"{feBaseUrl}/link-expired"),
                ErrorCodes.LinkDisabled => Redirect($"{feBaseUrl}/link-disabled"),
                ErrorCodes.ClickLimitReached => Redirect($"{feBaseUrl}/link-limit-reached"),
                ErrorCodes.PasswordRequired => Redirect($"{feBaseUrl}/link/{slug}/unlock"),
                _ => Redirect($"{feBaseUrl}/link-not-found"),
            };
        }
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
            var wrapper = await _redirectService.BuildWrapperRenderAsync(ResolveHost(), slug, redirectResponse.RedirectUrl, cancellationToken);
            if (wrapper is not null)
            {
                return Content(RenderWrapperHtml(wrapper), "text/html");
            }
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

    private static string RenderWrapperHtml(PublicWrapperRenderDto wrapper)
    {
        var title = System.Net.WebUtility.HtmlEncode(wrapper.Title);
        var description = System.Net.WebUtility.HtmlEncode(wrapper.Description);
        var image = string.IsNullOrWhiteSpace(wrapper.ImageUrl) ? string.Empty : System.Net.WebUtility.HtmlEncode(wrapper.ImageUrl);
        var buttonText = System.Net.WebUtility.HtmlEncode(wrapper.ButtonText);
        var warning = System.Net.WebUtility.HtmlEncode(wrapper.WarningText);
        var continueUrl = System.Net.WebUtility.HtmlEncode(wrapper.ContinueUrl);
        var theme = System.Net.WebUtility.HtmlEncode(wrapper.Theme);
        var brandName = System.Net.WebUtility.HtmlEncode(wrapper.BrandName ?? wrapper.Host);
        var brandLogo = string.IsNullOrWhiteSpace(wrapper.BrandLogoUrl) ? string.Empty : System.Net.WebUtility.HtmlEncode(wrapper.BrandLogoUrl);
        var ctaTitle = System.Net.WebUtility.HtmlEncode(wrapper.CtaTitle ?? string.Empty);
        var ctaDescription = System.Net.WebUtility.HtmlEncode(wrapper.CtaDescription ?? string.Empty);
        var ctaButtonText = System.Net.WebUtility.HtmlEncode(wrapper.CtaButtonText ?? string.Empty);
        var ctaButtonUrl = string.IsNullOrWhiteSpace(wrapper.CtaButtonUrl) ? string.Empty : System.Net.WebUtility.HtmlEncode(wrapper.CtaButtonUrl);
        var delaySeconds = Math.Max(1, wrapper.DelaySeconds);
        var isManual = wrapper.RedirectMode.Equals("ManualContinue", StringComparison.OrdinalIgnoreCase);
        var isLanding = wrapper.RedirectMode.Equals("LandingPage", StringComparison.OrdinalIgnoreCase);
        var effectiveDelay = isManual ? 0 : delaySeconds;
        var autoRedirectScript = effectiveDelay > 0
            ? $"let s={effectiveDelay};const el=document.getElementById('count');if(el){{el.textContent=s;}}setInterval(function(){{s--;if(el&&s>=0){{el.textContent=s;}}if(s<=0){{window.location.replace('{continueUrl}');}}}},1000);"
            : string.Empty;
        var imageBlock = string.IsNullOrWhiteSpace(image) ? string.Empty : $"<img class=\"hero\" src=\"{image}\" alt=\"wrapper image\">";
        var logoBlock = string.IsNullOrWhiteSpace(brandLogo) ? string.Empty : $"<img class=\"logo\" src=\"{brandLogo}\" alt=\"brand logo\">";
        var ctaBlock = isLanding && !string.IsNullOrWhiteSpace(ctaTitle)
            ? $"<div class=\"cta\"><strong>{ctaTitle}</strong><p>{ctaDescription}</p>{(string.IsNullOrWhiteSpace(ctaButtonUrl) ? string.Empty : $"<a class=\"cta-btn\" href=\"{ctaButtonUrl}\" target=\"_blank\" rel=\"noopener noreferrer\">{(string.IsNullOrWhiteSpace(ctaButtonText) ? "Xem them" : ctaButtonText)}</a>")}</div>"
            : string.Empty;
        var continueBlock = isManual || isLanding
            ? $"<a class=\"continue\" href=\"{continueUrl}\" rel=\"noopener noreferrer\">{buttonText}</a>"
            : $"<a class=\"continue ghost\" href=\"{continueUrl}\" rel=\"noopener noreferrer\">Mo ngay</a>";
        var countdownBlock = effectiveDelay > 0 ? "<div class=\"countdown\">Dang chuyen trong <span id=\"count\"></span> giay...</div>" : string.Empty;

        return "<!DOCTYPE html><html lang=\"vi\"><head>" +
            "<meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">" +
            $"<title>{title}</title>" +
            "<style>" +
            "body{margin:0;font-family:Inter,system-ui,sans-serif;background:#0f172a;color:#e2e8f0;display:flex;min-height:100vh;align-items:center;justify-content:center;padding:24px;}" +
            ".shell{width:min(100%,760px);background:rgba(15,23,42,.92);border:1px solid rgba(148,163,184,.2);border-radius:28px;overflow:hidden;box-shadow:0 24px 80px rgba(15,23,42,.45);}" +
            $".head{{padding:24px 28px;background:{(theme == "dark" ? "linear-gradient(135deg,#111827,#1f2937)" : "linear-gradient(135deg,#0ea5e9,#2563eb)")};}}" +
            ".brand{display:flex;align-items:center;gap:12px;font-weight:700;}" +
            ".logo{width:40px;height:40px;border-radius:999px;object-fit:cover;background:#fff;}" +
            ".body{padding:28px;display:flex;flex-direction:column;gap:18px;}" +
            ".hero{width:100%;max-height:280px;object-fit:cover;border-radius:18px;}" +
            ".title{font-size:28px;font-weight:800;color:#fff;margin:0;}" +
            ".desc{margin:0;color:#cbd5e1;line-height:1.6;}" +
            ".warn{padding:14px 16px;border-radius:14px;background:rgba(245,158,11,.12);border:1px solid rgba(245,158,11,.35);color:#fde68a;}" +
            ".countdown{font-weight:700;color:#93c5fd;}" +
            ".cta{padding:18px;border-radius:18px;background:rgba(30,41,59,.8);border:1px solid rgba(148,163,184,.14);}" +
            ".cta p{margin:8px 0 0;color:#cbd5e1;}" +
            ".cta-btn,.continue{display:inline-flex;align-items:center;justify-content:center;padding:14px 18px;border-radius:14px;text-decoration:none;font-weight:700;}" +
            ".cta-btn{margin-top:14px;background:#f59e0b;color:#111827;}" +
            ".continue{background:#38bdf8;color:#082f49;}" +
            ".ghost{background:rgba(56,189,248,.16);color:#bae6fd;}" +
            ".foot{padding:0 28px 28px;color:#94a3b8;font-size:13px;}" +
            "@media (max-width:640px){.head,.body,.foot{padding-left:18px;padding-right:18px;}.title{font-size:22px;}}" +
            "</style></head><body><div class=\"shell\"><div class=\"head\"><div class=\"brand\">" +
            logoBlock +
            $"<span>{brandName}</span></div></div><div class=\"body\">{imageBlock}<h1 class=\"title\">{title}</h1><p class=\"desc\">{description}</p>{countdownBlock}<div class=\"warn\">{warning}</div>{ctaBlock}{continueBlock}</div><div class=\"foot\">Ban sap roi khoi website hien tai va duoc chuyen den trang dich.</div></div><script>{autoRedirectScript}</script></body></html>";
    }
}
