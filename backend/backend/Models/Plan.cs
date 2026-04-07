public sealed class Plan
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public int LinkLimit { get; set; }
    public int AnalyticsRetentionDays { get; set; }
    public bool HasCustomDomain { get; set; }
    public bool HasAdvancedAnalytics { get; set; }
    public bool HasApiAccess { get; set; }
}
