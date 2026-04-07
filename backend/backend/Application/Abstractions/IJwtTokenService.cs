using WebShortlink.Backend.Domain.Entities;

namespace WebShortlink.Backend.Application.Abstractions;

public interface IJwtTokenService
{
    string CreateAccessToken(AppUser user, IList<string> roles);
    string CreateRefreshToken();
    string HashRefreshToken(string refreshToken);
}
