using System.Security.Claims;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;

namespace WebShortlink.Backend.Infrastructure.Auth;

public sealed class HttpCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUserContext GetRequired()
    {
        return GetCurrentOrNull() ?? throw new AppException(ErrorCodes.Unauthorized, "Phiên đăng nhập không hợp lệ.", StatusCodes.Status401Unauthorized);
    }

    public CurrentUserContext? GetCurrentOrNull()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var user = httpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            return null;
        }

        return new CurrentUserContext(
            parsedUserId,
            user.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            user.IsInRole(Domain.Enums.AppRoles.Admin),
            httpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown");
    }
}
