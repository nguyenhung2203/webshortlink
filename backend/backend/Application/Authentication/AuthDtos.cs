namespace WebShortlink.Backend.Application.Authentication;

public sealed record RegisterRequestDto(
    string FullName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? TurnstileToken);

public sealed record LoginRequestDto(
    string Email,
    string Password,
    string? TurnstileToken);

public sealed record RefreshTokenRequestDto(string RefreshToken);

public sealed record ForgotPasswordRequestDto(string Email, string? TurnstileToken);

public sealed record ResetPasswordRequestDto(string Email, string Token, string NewPassword, string ConfirmPassword);

public sealed record VerifyEmailRequestDto(string UserId, string Token);

public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAtUtc,
    SessionUserDto User);

public sealed record SessionUserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    int CurrentPlanId,
    string AccountStatus);
