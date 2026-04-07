using WebShortlink.Backend.Domain.Common;

namespace WebShortlink.Backend.Domain.Entities;

public class SystemSetting : AuditableEntity
{
    public Guid Id { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public bool IsSensitive { get; set; }
}
