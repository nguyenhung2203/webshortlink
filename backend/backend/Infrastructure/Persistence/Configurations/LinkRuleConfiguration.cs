using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebShortlink.Backend.Domain.Entities;

namespace WebShortlink.Backend.Infrastructure.Persistence.Configurations;

public sealed class LinkRuleConfiguration : IEntityTypeConfiguration<LinkRule>
{
    public void Configure(EntityTypeBuilder<LinkRule> builder)
    {
        builder.ToTable("LinkRules");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RuleValue).HasMaxLength(500).IsRequired();
        builder.Property(x => x.TargetUrl).HasMaxLength(2048).IsRequired();
        builder.HasIndex(x => new { x.LinkId, x.RuleType, x.Priority });
    }
}
