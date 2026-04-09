using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebShortlink.Backend.Domain.Entities;
using WebShortlink.Backend.Domain.Enums;

namespace WebShortlink.Backend.Infrastructure.Persistence.Seed;

public sealed class ApplicationRuntimeSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly Microsoft.Extensions.Options.IOptions<WebShortlink.Backend.Infrastructure.Options.AppOptions> _appOptions;

    public ApplicationRuntimeSeeder(
        ApplicationDbContext dbContext,
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<AppUser> userManager,
        Microsoft.Extensions.Options.IOptions<WebShortlink.Backend.Infrastructure.Options.AppOptions> appOptions)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
        _appOptions = appOptions;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureMigratedSafeAsync(cancellationToken);
        await SeedRolesAsync();
        await SeedPlansAsync(cancellationToken);
        await SeedSettingsAsync(cancellationToken);
        await EnsureAdminExistsAsync(cancellationToken);
    }

    private async Task EnsureMigratedSafeAsync(CancellationToken cancellationToken)
    {
        var pending = (await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
        if (pending.Count == 0)
        {
            return;
        }

        // Apply migrations safely. Do not fake migration history.
        // If there's a schema mismatch (tables exist but no history), this will intentionally throw
        // requiring developers to resolve it rather than blindly bypassing and risking runtime crashes.
        await _dbContext.Database.MigrateAsync(cancellationToken);
    }

    private async Task SeedRolesAsync()
    {
        foreach (var role in new[] { AppRoles.User, AppRoles.Admin })
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    private async Task SeedPlansAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var plans = new[]
        {
            new Plan { Id = 1, Code = "regular", Name = "Thường", MonthlyPrice = 0, IsActive = true, CreatedAtUtc = now },
            new Plan { Id = 2, Code = "pro", Name = "Pro", MonthlyPrice = 199000, IsActive = true, CreatedAtUtc = now },
            new Plan { Id = 3, Code = "plus", Name = "Plus", MonthlyPrice = 499000, IsActive = true, CreatedAtUtc = now }
        };

        foreach (var plan in plans)
        {
            var p = await _dbContext.Plans.FirstOrDefaultAsync(x => x.Id == plan.Id, cancellationToken);
            if (p == null) _dbContext.Plans.Add(plan);
        }

        if (_dbContext.ChangeTracker.HasChanges())
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try {
                await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Plans ON", cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Plans OFF", cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            } catch {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        var features = new[]
        {
            new PlanFeature { Id = 1, PlanId = 1, FeatureKey = "links.max_count", LimitValue = 100, IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 2, PlanId = 2, FeatureKey = "links.max_count", LimitValue = 5000, IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 3, PlanId = 3, FeatureKey = "links.max_count", LimitValue = 50000, IsEnabled = true, CreatedAtUtc = now },

            new PlanFeature { Id = 4, PlanId = 1, FeatureKey = "domains.custom", IsEnabled = false, CreatedAtUtc = now },
            new PlanFeature { Id = 5, PlanId = 2, FeatureKey = "domains.custom", IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 6, PlanId = 3, FeatureKey = "domains.custom", IsEnabled = true, CreatedAtUtc = now },

            new PlanFeature { Id = 7, PlanId = 1, FeatureKey = "analytics.advanced", IsEnabled = false, CreatedAtUtc = now },
            new PlanFeature { Id = 8, PlanId = 2, FeatureKey = "analytics.advanced", IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 9, PlanId = 3, FeatureKey = "analytics.advanced", IsEnabled = true, CreatedAtUtc = now },

            new PlanFeature { Id = 10, PlanId = 1, FeatureKey = "links.custom_slug", IsEnabled = false, CreatedAtUtc = now },
            new PlanFeature { Id = 11, PlanId = 2, FeatureKey = "links.custom_slug", IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 12, PlanId = 3, FeatureKey = "links.custom_slug", IsEnabled = true, CreatedAtUtc = now },

            new PlanFeature { Id = 13, PlanId = 1, FeatureKey = "domains.max_count", LimitValue = 0, IsEnabled = false, CreatedAtUtc = now },
            new PlanFeature { Id = 14, PlanId = 2, FeatureKey = "domains.max_count", LimitValue = 3, IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 15, PlanId = 3, FeatureKey = "domains.max_count", LimitValue = 20, IsEnabled = true, CreatedAtUtc = now },

            new PlanFeature { Id = 16, PlanId = 1, FeatureKey = "analytics.retention_days", LimitValue = 30, IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 17, PlanId = 2, FeatureKey = "analytics.retention_days", LimitValue = 180, IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 18, PlanId = 3, FeatureKey = "analytics.retention_days", LimitValue = 730, IsEnabled = true, CreatedAtUtc = now },

            new PlanFeature { Id = 19, PlanId = 1, FeatureKey = "links.password_protection", IsEnabled = false, CreatedAtUtc = now },
            new PlanFeature { Id = 20, PlanId = 2, FeatureKey = "links.password_protection", IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 21, PlanId = 3, FeatureKey = "links.password_protection", IsEnabled = true, CreatedAtUtc = now },

            new PlanFeature { Id = 22, PlanId = 1, FeatureKey = "links.expiration", IsEnabled = false, CreatedAtUtc = now },
            new PlanFeature { Id = 23, PlanId = 2, FeatureKey = "links.expiration", IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 24, PlanId = 3, FeatureKey = "links.expiration", IsEnabled = true, CreatedAtUtc = now },

            new PlanFeature { Id = 25, PlanId = 1, FeatureKey = "links.click_limit", IsEnabled = false, CreatedAtUtc = now },
            new PlanFeature { Id = 26, PlanId = 2, FeatureKey = "links.click_limit", IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 27, PlanId = 3, FeatureKey = "links.click_limit", IsEnabled = true, CreatedAtUtc = now },

            new PlanFeature { Id = 28, PlanId = 1, FeatureKey = "rules.targeting_advanced", IsEnabled = false, CreatedAtUtc = now },
            new PlanFeature { Id = 29, PlanId = 2, FeatureKey = "rules.targeting_advanced", IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 30, PlanId = 3, FeatureKey = "rules.targeting_advanced", IsEnabled = true, CreatedAtUtc = now },

            new PlanFeature { Id = 31, PlanId = 1, FeatureKey = "reports.export", IsEnabled = false, CreatedAtUtc = now },
            new PlanFeature { Id = 32, PlanId = 2, FeatureKey = "reports.export", IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 33, PlanId = 3, FeatureKey = "reports.export", IsEnabled = true, CreatedAtUtc = now },

            new PlanFeature { Id = 34, PlanId = 1, FeatureKey = "links.social_preview", IsEnabled = false, CreatedAtUtc = now },
            new PlanFeature { Id = 35, PlanId = 2, FeatureKey = "links.social_preview", IsEnabled = true, CreatedAtUtc = now },
            new PlanFeature { Id = 36, PlanId = 3, FeatureKey = "links.social_preview", IsEnabled = true, CreatedAtUtc = now }
        };

        foreach (var f in features)
        {
            var existing = await _dbContext.PlanFeatures.FirstOrDefaultAsync(x => x.PlanId == f.PlanId && x.FeatureKey == f.FeatureKey, cancellationToken);
            if (existing is null)
            {
                _dbContext.PlanFeatures.Add(f);
            }
            else
            {
                existing.IsEnabled = f.IsEnabled;
                existing.LimitValue = f.LimitValue;
            }
        }

        if (_dbContext.ChangeTracker.HasChanges())
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try {
                await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT PlanFeatures ON", cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT PlanFeatures OFF", cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            } catch {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }

    private async Task SeedSettingsAsync(CancellationToken cancellationToken)
    {
        if (await _dbContext.SystemSettings.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;
        _dbContext.SystemSettings.AddRange(
            new SystemSetting { Id = Guid.NewGuid(), GroupName = "Retention", SettingKey = "analytics.raw_days", SettingValue = "90", CreatedAtUtc = now },
            new SystemSetting { Id = Guid.NewGuid(), GroupName = "Retention", SettingKey = "analytics.aggregate_days", SettingValue = "730", CreatedAtUtc = now },
            new SystemSetting { Id = Guid.NewGuid(), GroupName = "Security", SettingKey = "link.mask_ip_for_user", SettingValue = "true", CreatedAtUtc = now },
            new SystemSetting { Id = Guid.NewGuid(), GroupName = "Limits", SettingKey = "redirect.default_rate_limit_per_minute", SettingValue = "600", CreatedAtUtc = now });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureAdminExistsAsync(CancellationToken cancellationToken)
    {
        var adminEmail = _appOptions.Value.AdminEmail;
        var adminPassword = _appOptions.Value.AdminPassword;
        
        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            // Do not construct a default admin with a hardcoded dictionary generic password.
            // A missing password simply skips admin bootstrap execution.
            return;
        }

        var admin = await _userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                EmailConfirmed = true,
                AccountStatus = UserAccountStatus.Active,
                CurrentPlanId = 3,
                CreatedAtUtc = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(admin, adminPassword);
            if (!result.Succeeded)
            {
                // In production it's better to fail fast or log an error than to stay silent
                throw new InvalidOperationException($"Failed to bootstrap admin: {string.Join(", ", result.Errors.Select(x => x.Description))}");
            }
        }

        if (!await _userManager.IsInRoleAsync(admin, AppRoles.Admin))
        {
            await _userManager.AddToRoleAsync(admin, AppRoles.Admin);
        }
    }

}
