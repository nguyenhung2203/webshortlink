namespace WebShortlink.Backend.Infrastructure.Options;

public class AppOptions
{
    public const string SectionName = "App";
    
    public string FrontendUrl { get; set; } = "http://localhost:5173";
    public string DefaultDomain { get; set; } = "sho.rt";
    public string AdminEmail { get; set; } = "admin@demo.local";
    public string? AdminPassword { get; set; }
}
