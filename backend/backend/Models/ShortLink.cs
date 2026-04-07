public sealed class ShortLink
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortUrl { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string? Password { get; set; }
    public DateTime? ExpiredAtUtc { get; set; }
    public int? ClickLimit { get; set; }
    public string Tag { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
