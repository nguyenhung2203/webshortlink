namespace WebShortlink.Backend.Application.Domains;

public sealed record CreateDomainRequestDto(string Host, DateTime? ExpiredAtUtc, string? UserNotes);

public sealed record VerifyDomainRequestDto(string VerificationToken);

public sealed record VerifyDomainResponseDto(
    bool Verified,
    string Host,
    string Message);

public sealed record DomainListItemDto(
    Guid Id,
    string Host,
    bool IsVerified,
    string? AdminFeedback,
    DateTime? VerifiedAtUtc,
    DateTime CreatedAtUtc,
    DateTime? ExpiredAtUtc,
    string? UserNotes,
    bool IsDefault = false);
