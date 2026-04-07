namespace WebShortlink.Backend.Domain.Enums;

public enum ClickEventStatus
{
    Redirected = 1,
    PasswordRequired = 2,
    Disabled = 3,
    Expired = 4,
    ClickLimitReached = 5,
    NotFound = 6
}
