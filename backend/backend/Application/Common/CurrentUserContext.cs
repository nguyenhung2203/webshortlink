namespace WebShortlink.Backend.Application.Common;

public sealed record CurrentUserContext(Guid UserId, string Email, bool IsAdmin, string IpAddress);
