namespace WebShortlink.Backend.Application.Domains;

public sealed record CreateDomainRequestDto(string Host);

public sealed record VerifyDomainRequestDto(string VerificationToken);

public sealed record VerifyDomainResponseDto(
    bool Verified,
    string Host,
    string Message);

public sealed record DomainListItemDto(
    Guid Id,
    string Host,
    bool IsVerified,
    string VerificationToken,
    DateTime? VerifiedAtUtc,
    DateTime CreatedAtUtc,
    bool IsDefault = false);
