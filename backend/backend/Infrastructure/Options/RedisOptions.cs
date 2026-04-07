namespace WebShortlink.Backend.Infrastructure.Options;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";

    public bool Enabled { get; set; } = true;
    public string Configuration { get; set; } = "localhost:6379";
    public string InstanceName { get; set; } = "webshortlink:";
    public string ClickQueueKey { get; set; } = "analytics:click-queue";
    public int HotLinkTtlMinutes { get; set; } = 30;
    public int DashboardTtlMinutes { get; set; } = 5;
}
