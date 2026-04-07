namespace WebShortlink.Backend.Application.Common;

public static class ErrorCodes
{
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string NotFound = "NOT_FOUND";
    public const string Conflict = "CONFLICT";
    public const string TurnstileFailed = "TURNSTILE_FAILED";
    public const string LinkDisabled = "LINK_DISABLED";
    public const string LinkExpired = "LINK_EXPIRED";
    public const string ClickLimitReached = "CLICK_LIMIT_REACHED";
    public const string PasswordRequired = "PASSWORD_REQUIRED";
    public const string InvalidPassword = "INVALID_PASSWORD";
    public const string PlanFeatureDenied = "PLAN_FEATURE_DENIED";
    public const string RateLimited = "RATE_LIMITED";
}
