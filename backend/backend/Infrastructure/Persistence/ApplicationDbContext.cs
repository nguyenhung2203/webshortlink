using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Domain.Entities;

namespace WebShortlink.Backend.Infrastructure.Persistence;

public sealed class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<CustomDomain> Domains => Set<CustomDomain>();
    public DbSet<Link> Links => Set<Link>();
    public DbSet<LinkRule> LinkRules => Set<LinkRule>();
    public DbSet<ClickEvent> ClickEvents => Set<ClickEvent>();
    public DbSet<LinkDailyStat> LinkDailyStats => Set<LinkDailyStat>();
    public DbSet<LinkHourlyStat> LinkHourlyStats => Set<LinkHourlyStat>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AbuseReport> AbuseReports => Set<AbuseReport>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
