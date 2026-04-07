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

    public ApplicationRuntimeSeeder(
        ApplicationDbContext dbContext,
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureMigratedSafeAsync(cancellationToken);
        await SeedRolesAsync();
        await SeedPlansAsync(cancellationToken);
        await SeedSettingsAsync(cancellationToken);
        await SeedDemoUsersAsync(cancellationToken);
    }

    private async Task EnsureMigratedSafeAsync(CancellationToken cancellationToken)
    {
        var pending = (await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
        if (pending.Count == 0)
        {
            return;
        }

        var connection = _dbContext.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);

        await using var tableCheckCommand = connection.CreateCommand();
        tableCheckCommand.CommandText = "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AppUsers'";
        var appUsersTableExists = (int)(await tableCheckCommand.ExecuteScalarAsync(cancellationToken) ?? 0) > 0;

        if (appUsersTableExists)
        {
            await using var historyCommand = connection.CreateCommand();
            historyCommand.CommandText =
                """
                IF OBJECT_ID(N'[__EFMigrationsHistory]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [__EFMigrationsHistory] (
                        [MigrationId] nvarchar(150) NOT NULL,
                        [ProductVersion] nvarchar(32) NOT NULL,
                        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                    );
                END
                """;
            await historyCommand.ExecuteNonQueryAsync(cancellationToken);
            await connection.CloseAsync();

            var appliedMigrations = (await _dbContext.Database.GetAppliedMigrationsAsync(cancellationToken)).ToHashSet();
            foreach (var migration in pending.Where(migration => !appliedMigrations.Contains(migration)))
            {
                await _dbContext.Database.ExecuteSqlRawAsync(
                    "INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ({0}, '8.0.14')",
                    migration);
            }

            return;
        }

        await connection.CloseAsync();
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
        await _dbContext.SaveChangesAsync(cancellationToken);

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
            new PlanFeature { Id = 33, PlanId = 3, FeatureKey = "reports.export", IsEnabled = true, CreatedAtUtc = now }
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

        await _dbContext.SaveChangesAsync(cancellationToken);
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

    private async Task SeedDemoUsersAsync(CancellationToken cancellationToken)
    {
        var admin = await _userManager.FindByEmailAsync("admin@demo.local");
        if (admin is null)
        {
            admin = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = "admin@demo.local",
                Email = "admin@demo.local",
                FullName = "System Admin",
                EmailConfirmed = true,
                AccountStatus = UserAccountStatus.Active,
                CurrentPlanId = 3,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _userManager.CreateAsync(admin, "Admin123!");
        }

        if (!await _userManager.IsInRoleAsync(admin, AppRoles.Admin))
        {
            await _userManager.AddToRoleAsync(admin, AppRoles.Admin);
        }

        await EnsureSubscriptionAsync(admin, 3, cancellationToken);

        var user = await _userManager.FindByEmailAsync("user@demo.local");
        if (user is null)
        {
            user = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = "user@demo.local",
                Email = "user@demo.local",
                FullName = "Demo User",
                EmailConfirmed = true,
                AccountStatus = UserAccountStatus.Active,
                CurrentPlanId = 2,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _userManager.CreateAsync(user, "Demo123!");
        }

        if (!await _userManager.IsInRoleAsync(user, AppRoles.User))
        {
            await _userManager.AddToRoleAsync(user, AppRoles.User);
        }

        await EnsureSubscriptionAsync(user, 2, cancellationToken);
        await EnsureDemoPaymentAsync(user, cancellationToken);
        await EnsureDemoLinkAsync(user, cancellationToken);
    }

    private async Task EnsureSubscriptionAsync(AppUser user, int planId, CancellationToken cancellationToken)
    {
        user.CurrentPlanId = planId;
        var existingSubscriptions = await _dbContext.Subscriptions
            .Where(x => x.UserId == user.Id && x.Status == SubscriptionStatus.Active)
            .ToListAsync(cancellationToken);

        foreach (var existingSubscription in existingSubscriptions.Where(x => x.PlanId != planId))
        {
            existingSubscription.Status = SubscriptionStatus.Expired;
            existingSubscription.EndAtUtc = DateTime.UtcNow;
            existingSubscription.UpdatedAtUtc = DateTime.UtcNow;
        }

        var hasActiveSubscription = existingSubscriptions.Any(x => x.PlanId == planId && x.Status == SubscriptionStatus.Active);

        if (!hasActiveSubscription)
        {
            _dbContext.Subscriptions.Add(new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                PlanId = planId,
                Status = SubscriptionStatus.Active,
                StartAtUtc = DateTime.UtcNow.AddDays(-7),
                EndAtUtc = DateTime.UtcNow.AddMonths(1),
                CreatedAtUtc = DateTime.UtcNow.AddDays(-7)
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureDemoPaymentAsync(AppUser user, CancellationToken cancellationToken)
    {
        var subscription = await _dbContext.Subscriptions
            .OrderByDescending(x => x.StartAtUtc)
            .FirstAsync(x => x.UserId == user.Id && x.Status == SubscriptionStatus.Active, cancellationToken);

        var hasPayment = await _dbContext.Payments.AnyAsync(x => x.SubscriptionId == subscription.Id, cancellationToken);
        if (hasPayment)
        {
            return;
        }

        _dbContext.Payments.Add(new Payment
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription.Id,
            Amount = 199000,
            Currency = "VND",
            Provider = "demo",
            ProviderReference = $"demo-{subscription.Id:N}",
            Status = PaymentStatus.Paid,
            PaidAtUtc = DateTime.UtcNow.AddDays(-3),
            CreatedAtUtc = DateTime.UtcNow.AddDays(-3)
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureDemoLinkAsync(AppUser user, CancellationToken cancellationToken)
    {
        var hasLink = await _dbContext.Links.AnyAsync(x => x.UserId == user.Id && !x.IsDeleted, cancellationToken);
        if (hasLink)
        {
            return;
        }

        _dbContext.Links.Add(new Link
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Slug = "demo123",
            OriginalUrl = "https://example.com/welcome",
            Description = "Demo link",
            Tag = "seed",
            Status = LinkStatus.Active,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-2),
            TotalClicks = 0,
            UniqueClicks = 0
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
