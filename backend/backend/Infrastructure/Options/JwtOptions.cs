namespace WebShortlink.Backend.Infrastructure.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "WebShortlink";
    public string Audience { get; set; } = "WebShortlink.Frontend";
    public string SigningKey { get; set; } = "ChangeThisSigningKeyForProductionAtLeast32Chars!";
    public int AccessTokenMinutes { get; set; } = 30;
    public int RefreshTokenDays { get; set; } = 30;
}
