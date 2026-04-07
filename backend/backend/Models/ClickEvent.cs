public sealed class ClickEvent
{
    public Guid Id { get; set; }
    public Guid LinkId { get; set; }
    public DateTime ClickedAtUtc { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string MaskedIp { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string Referrer { get; set; } = "direct";
    public string UserAgent { get; set; } = string.Empty;
    public bool IsBot { get; set; }
    public bool IsSuspicious { get; set; }
    public string RedirectStatus { get; set; } = "302";
}
