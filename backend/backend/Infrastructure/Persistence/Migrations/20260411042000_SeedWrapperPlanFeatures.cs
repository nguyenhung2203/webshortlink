using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebShortlink.Backend.Infrastructure.Persistence.Migrations
{
    public partial class SeedWrapperPlanFeatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
IF NOT EXISTS (SELECT 1 FROM [PlanFeatures] WHERE [PlanId] = 1 AND [FeatureKey] = N'links.wrapper')
    INSERT INTO [PlanFeatures] ([Id], [PlanId], [FeatureKey], [FeatureValue], [LimitValue], [IsEnabled], [CreatedAtUtc], [UpdatedAtUtc], [CreatedByUserId], [UpdatedByUserId])
    VALUES (37, 1, N'links.wrapper', NULL, NULL, 0, SYSUTCDATETIME(), NULL, N'system', NULL);
IF NOT EXISTS (SELECT 1 FROM [PlanFeatures] WHERE [PlanId] = 2 AND [FeatureKey] = N'links.wrapper')
    INSERT INTO [PlanFeatures] ([Id], [PlanId], [FeatureKey], [FeatureValue], [LimitValue], [IsEnabled], [CreatedAtUtc], [UpdatedAtUtc], [CreatedByUserId], [UpdatedByUserId])
    VALUES (38, 2, N'links.wrapper', NULL, NULL, 1, SYSUTCDATETIME(), NULL, N'system', NULL);
IF NOT EXISTS (SELECT 1 FROM [PlanFeatures] WHERE [PlanId] = 3 AND [FeatureKey] = N'links.wrapper')
    INSERT INTO [PlanFeatures] ([Id], [PlanId], [FeatureKey], [FeatureValue], [LimitValue], [IsEnabled], [CreatedAtUtc], [UpdatedAtUtc], [CreatedByUserId], [UpdatedByUserId])
    VALUES (39, 3, N'links.wrapper', NULL, NULL, 1, SYSUTCDATETIME(), NULL, N'system', NULL);

IF NOT EXISTS (SELECT 1 FROM [PlanFeatures] WHERE [PlanId] = 1 AND [FeatureKey] = N'links.wrapper_landing')
    INSERT INTO [PlanFeatures] ([Id], [PlanId], [FeatureKey], [FeatureValue], [LimitValue], [IsEnabled], [CreatedAtUtc], [UpdatedAtUtc], [CreatedByUserId], [UpdatedByUserId])
    VALUES (40, 1, N'links.wrapper_landing', NULL, NULL, 0, SYSUTCDATETIME(), NULL, N'system', NULL);
IF NOT EXISTS (SELECT 1 FROM [PlanFeatures] WHERE [PlanId] = 2 AND [FeatureKey] = N'links.wrapper_landing')
    INSERT INTO [PlanFeatures] ([Id], [PlanId], [FeatureKey], [FeatureValue], [LimitValue], [IsEnabled], [CreatedAtUtc], [UpdatedAtUtc], [CreatedByUserId], [UpdatedByUserId])
    VALUES (41, 2, N'links.wrapper_landing', NULL, NULL, 0, SYSUTCDATETIME(), NULL, N'system', NULL);
IF NOT EXISTS (SELECT 1 FROM [PlanFeatures] WHERE [PlanId] = 3 AND [FeatureKey] = N'links.wrapper_landing')
    INSERT INTO [PlanFeatures] ([Id], [PlanId], [FeatureKey], [FeatureValue], [LimitValue], [IsEnabled], [CreatedAtUtc], [UpdatedAtUtc], [CreatedByUserId], [UpdatedByUserId])
    VALUES (42, 3, N'links.wrapper_landing', NULL, NULL, 1, SYSUTCDATETIME(), NULL, N'system', NULL);

IF NOT EXISTS (SELECT 1 FROM [PlanFeatures] WHERE [PlanId] = 1 AND [FeatureKey] = N'links.wrapper_cta')
    INSERT INTO [PlanFeatures] ([Id], [PlanId], [FeatureKey], [FeatureValue], [LimitValue], [IsEnabled], [CreatedAtUtc], [UpdatedAtUtc], [CreatedByUserId], [UpdatedByUserId])
    VALUES (43, 1, N'links.wrapper_cta', NULL, NULL, 0, SYSUTCDATETIME(), NULL, N'system', NULL);
IF NOT EXISTS (SELECT 1 FROM [PlanFeatures] WHERE [PlanId] = 2 AND [FeatureKey] = N'links.wrapper_cta')
    INSERT INTO [PlanFeatures] ([Id], [PlanId], [FeatureKey], [FeatureValue], [LimitValue], [IsEnabled], [CreatedAtUtc], [UpdatedAtUtc], [CreatedByUserId], [UpdatedByUserId])
    VALUES (44, 2, N'links.wrapper_cta', NULL, NULL, 0, SYSUTCDATETIME(), NULL, N'system', NULL);
IF NOT EXISTS (SELECT 1 FROM [PlanFeatures] WHERE [PlanId] = 3 AND [FeatureKey] = N'links.wrapper_cta')
    INSERT INTO [PlanFeatures] ([Id], [PlanId], [FeatureKey], [FeatureValue], [LimitValue], [IsEnabled], [CreatedAtUtc], [UpdatedAtUtc], [CreatedByUserId], [UpdatedByUserId])
    VALUES (45, 3, N'links.wrapper_cta', NULL, NULL, 1, SYSUTCDATETIME(), NULL, N'system', NULL);
""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
DELETE FROM [PlanFeatures]
WHERE [FeatureKey] IN (N'links.wrapper', N'links.wrapper_landing', N'links.wrapper_cta')
  AND [Id] BETWEEN 37 AND 45;
""");
        }
    }
}
