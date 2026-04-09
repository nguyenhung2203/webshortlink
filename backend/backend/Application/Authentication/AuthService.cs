using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebShortlink.Backend.Application.Abstractions;
using WebShortlink.Backend.Application.Common;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;
using WebShortlink.Backend.Infrastructure.Options;
using WebShortlink.Backend.Infrastructure.Persistence;

namespace WebShortlink.Backend.Application.Authentication;

public sealed class AuthService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITurnstileService _turnstileService;
    private readonly IAuditLogService _auditLogService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailSenderService _emailSenderService;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        ApplicationDbContext dbContext,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtTokenService jwtTokenService,
        ITurnstileService turnstileService,
        IAuditLogService auditLogService,
        ICurrentUserService currentUserService,
        IEmailSenderService emailSenderService,
        IOptions<JwtOptions> jwtOptions)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _turnstileService = turnstileService;
        _auditLogService = auditLogService;
        _currentUserService = currentUserService;
        _emailSenderService = emailSenderService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        ValidateRegister(request);
        await _turnstileService.VerifyAsync(request.TurnstileToken, _currentUserService.GetCurrentOrNull()?.IpAddress, cancellationToken);

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            throw new AppException(ErrorCodes.Conflict, "Email đã tồn tại.", StatusCodes.Status409Conflict);
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email.Trim().ToLowerInvariant(),
            Email = request.Email.Trim().ToLowerInvariant(),
            FullName = request.FullName.Trim(),
            AccountStatus = UserAccountStatus.Active,
            EmailConfirmed = false,
            CreatedAtUtc = DateTime.UtcNow,
            CurrentPlanId = 1
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new AppException(ErrorCodes.ValidationFailed, string.Join("; ", result.Errors.Select(x => x.Description)));
        }

        await _userManager.AddToRoleAsync(user, AppRoles.User);

        _dbContext.Subscriptions.Add(new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            PlanId = 1,
            Status = SubscriptionStatus.Active,
            StartAtUtc = DateTime.UtcNow,
            EndAtUtc = DateTime.UtcNow.AddMonths(1),
            CreatedAtUtc = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        var verificationToken = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultEmailProvider, "ConfirmEmail");
        await _emailSenderService.SendEmailAsync(
            user.Email,
            "Xác thực Tài khoản WeShort",
            $"Mã bí mật (OTP) để xác thực email của bạn là:\n\n{verificationToken}\n\nVui lòng copy và dán vào Hệ thống để kích hoạt quyền truy cập.",
            cancellationToken);

        await _auditLogService.WriteAsync(
            AuditActorType.Public,
            "PUB-API-001:Register",
            "AppUser",
            user.Id.ToString(),
            null,
            null,
            new { verificationToken = "***HIDE***", request.Email },
            cancellationToken);

        return new RegisterResponseDto(
            "Đăng ký thành công. Vui lòng xác thực email trước khi đăng nhập.",
            "EMAIL_VERIFICATION_REQUIRED",
            user.Id,
            user.Email ?? request.Email.Trim().ToLowerInvariant());
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        await _turnstileService.VerifyAsync(request.TurnstileToken, _currentUserService.GetCurrentOrNull()?.IpAddress, cancellationToken);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user is null)
        {
            await WriteLoginFailedAuditAsync(normalizedEmail, null, "UserNotFound", cancellationToken);
            throw new AppException(ErrorCodes.Unauthorized, "Sai email hoặc mật khẩu.", StatusCodes.Status401Unauthorized);
        }

        if (user.AccountStatus == UserAccountStatus.Locked || user.AccountStatus == UserAccountStatus.Disabled)
        {
            await WriteLoginFailedAuditAsync(normalizedEmail, user, "AccountInactive", cancellationToken);
            throw new AppException(ErrorCodes.Forbidden, "Tài khoản đã bị khóa.", StatusCodes.Status403Forbidden);
        }

        if (!user.EmailConfirmed)
        {
            throw new AppException(ErrorCodes.Forbidden, "Tài khoản chưa xác thực email.", StatusCodes.Status403Forbidden);
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!signInResult.Succeeded)
        {
            await WriteLoginFailedAuditAsync(normalizedEmail, user, "InvalidPassword", cancellationToken);
            throw new AppException(ErrorCodes.Unauthorized, "Sai email hoặc mật khẩu.", StatusCodes.Status401Unauthorized);
        }

        user.LastLoginAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponseDto> RefreshAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var tokenHash = _jwtTokenService.HashRefreshToken(request.RefreshToken);
        var session = await _dbContext.RefreshSessions
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.RefreshTokenHash == tokenHash && x.RevokedAtUtc == null, cancellationToken);

        if (session is null || session.ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new AppException(ErrorCodes.Unauthorized, "Refresh token không hợp lệ.", StatusCodes.Status401Unauthorized);
        }

        if (session.User.AccountStatus is UserAccountStatus.Locked or UserAccountStatus.Disabled)
        {
            session.RevokedAtUtc = DateTime.UtcNow;
            session.RevokedReason = "User account is no longer active";
            session.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);

            throw new AppException(ErrorCodes.Forbidden, "Tài khoản đã bị khóa.", StatusCodes.Status403Forbidden);
        }

        session.RevokedAtUtc = DateTime.UtcNow;
        session.RevokedReason = "Rotated";
        session.UpdatedAtUtc = DateTime.UtcNow;
        return await CreateAuthResponseAsync(session.User, cancellationToken);
    }

    public async Task<MessageResponseDto> LogoutAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var tokenHash = _jwtTokenService.HashRefreshToken(request.RefreshToken);
        var session = await _dbContext.RefreshSessions.FirstOrDefaultAsync(x => x.RefreshTokenHash == tokenHash, cancellationToken);
        if (session is not null)
        {
            session.RevokedAtUtc = DateTime.UtcNow;
            session.RevokedReason = "Logout";
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new MessageResponseDto("Đăng xuất thành công.");
    }

    public async Task<MessageResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken)
    {
        await _turnstileService.VerifyAsync(request.TurnstileToken, _currentUserService.GetCurrentOrNull()?.IpAddress, cancellationToken);
        var user = await _userManager.FindByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (user is null)
        {
            return new MessageResponseDto("Nếu email tồn tại, hệ thống sẽ gửi hướng dẫn đặt lại mật khẩu.");
        }

        var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultEmailProvider, "ResetPassword");
        await _emailSenderService.SendEmailAsync(
            user.Email,
            "Khôi phục Mật khẩu - WeShort",
            $"Hệ thống đã nhận được yêu cầu khôi phục mật khẩu. Mã bảo mật (OTP) của bạn là:\n\n{token}\n\nTuyệt đối không chia sẻ mã này cho bất kỳ ai.",
            cancellationToken);

        await _auditLogService.WriteAsync(
            AuditActorType.Public,
            "PUB-API-005:ForgotPassword",
            "AppUser",
            user.Id.ToString(),
            null,
            null,
            new { resetToken = "***HIDE***", request.Email },
            cancellationToken);

        return new MessageResponseDto("Nếu email tồn tại, hệ thống sẽ gửi hướng dẫn đặt lại mật khẩu.");
    }

    public async Task<MessageResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Xác nhận mật khẩu không khớp.");
        }

        var user = await _userManager.FindByEmailAsync(request.Email.Trim().ToLowerInvariant())
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy tài khoản.", StatusCodes.Status404NotFound);

        var isValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider, "ResetPassword", request.Token);
        if (!isValid)
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Mã OTP không hợp lệ hoặc đã hết hạn.");
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
        if (!result.Succeeded)
        {
            throw new AppException(ErrorCodes.ValidationFailed, string.Join("; ", result.Errors.Select(x => x.Description)));
        }

        return new MessageResponseDto("Đặt lại mật khẩu thành công.");
    }

    public async Task<MessageResponseDto> VerifyEmailAsync(VerifyEmailRequestDto request)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
        {
            throw new AppException(ErrorCodes.ValidationFailed, "UserId không hợp lệ.");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new AppException(ErrorCodes.NotFound, "Không tìm thấy tài khoản.", StatusCodes.Status404NotFound);

        var isValid = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider, "ConfirmEmail", request.Token);
        if (!isValid)
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Mã OTP xác thực không hợp lệ hoặc đã hết hạn.");
        }

        var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var result = await _userManager.ConfirmEmailAsync(user, confirmToken);
        if (!result.Succeeded)
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Token xác thực email không hợp lệ.");
        }

        user.AccountStatus = UserAccountStatus.Active;
        await _userManager.UpdateAsync(user);
        return new MessageResponseDto("Xác thực email thành công.");
    }

    public async Task<CurrentSessionDto> GetCurrentSessionAsync(CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetRequired();
        var user = await _dbContext.Users.AsNoTracking().FirstAsync(x => x.Id == current.UserId, cancellationToken);
        return new CurrentSessionDto(user.Id, user.Email!, user.FullName, current.IsAdmin ? AppRoles.Admin : AppRoles.User, user.CurrentPlanId, user.AccountStatus.ToString());
    }

    private async Task WriteLoginFailedAuditAsync(string email, AppUser? user, string reason, CancellationToken cancellationToken)
    {
        var current = _currentUserService.GetCurrentOrNull();

        await _auditLogService.WriteAsync(
            AuditActorType.Public,
            "PUB-API-002:LoginFailed",
            "AppUser",
            user?.Id.ToString(),
            user?.Id,
            current?.IpAddress,
            new
            {
                email,
                reason
            },
            cancellationToken);
    }

    private async Task<AuthResponseDto> CreateAuthResponseAsync(AppUser user, CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.CreateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.CreateRefreshToken();
        var refreshHash = _jwtTokenService.HashRefreshToken(refreshToken);

        _dbContext.RefreshSessions.Add(new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RefreshTokenHash = refreshHash,
            DeviceInfo = "spa",
            IpAddress = _currentUserService.GetCurrentOrNull()?.IpAddress ?? "unknown",
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays),
            CreatedAtUtc = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes),
            new SessionUserDto(
                user.Id,
                user.Email ?? string.Empty,
                user.FullName,
                roles.FirstOrDefault() ?? AppRoles.User,
                user.CurrentPlanId,
                user.AccountStatus.ToString()));
    }

    private static void ValidateRegister(RegisterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) || request.FullName.Length > 200)
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Họ tên không hợp lệ.");
        }

        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            throw new AppException(ErrorCodes.ValidationFailed, "Xác nhận mật khẩu không khớp.");
        }
    }
}
