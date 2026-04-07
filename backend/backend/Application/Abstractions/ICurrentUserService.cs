using WebShortlink.Backend.Application.Common;

namespace WebShortlink.Backend.Application.Abstractions;

public interface ICurrentUserService
{
    CurrentUserContext GetRequired();
    CurrentUserContext? GetCurrentOrNull();
}
