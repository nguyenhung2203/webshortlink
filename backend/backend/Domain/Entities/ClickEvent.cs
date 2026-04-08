using WebShortlink.Backend.Domain.Common;
using WebShortlink.Backend.Domain.Enums;

namespace WebShortlink.Backend.Domain.Entities;

public class ClickEvent : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid LinkId { get; set; }
    public DateTime ClickedAtUtc { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string MaskedIp { get; set; } = string.Empty;
    public string? CountryCode { get; set; }
    public string? City { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string Referrer { get; set; } = "direct";
    public string UserAgent { get; set; } = string.Empty;
    public string FingerprintHash { get; set; } = string.Empty;
    public ClickEventStatus EventStatus { get; set; } = ClickEventStatus.Redirected;
    public bool IsBot { get; set; }
    public bool IsSuspicious { get; set; }
    public int ResponseTimeMs { get; set; }
    public string NormalizedSource { get; set; } = "Direct";
    public string? UtmSource { get; set; }
    public string? UtmMedium { get; set; }
    public string? UtmCampaign { get; set; }

    public Link Link { get; set; } = null!;
}
